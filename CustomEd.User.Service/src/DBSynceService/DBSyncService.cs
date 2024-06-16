using System;
using System.Threading;
using System.Threading.Tasks;
using CustomEd.Shared.Data.Interfaces;
using CustomEd.User.Service.Model;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

public class DatabaseSyncService : IHostedService, IDisposable
{
    private readonly ILogger<DatabaseSyncService> _logger;
    private readonly UserApiClient _userApiClient;
    private readonly IGenericRepository<Admin> _adminRepository;
    private readonly IGenericRepository<Teacher> _teacherRepository;
    private readonly IGenericRepository<Student> _studentRepository;
    private Timer _timer;


    public DatabaseSyncService(ILogger<DatabaseSyncService> logger, UserApiClient userApiClient, IGenericRepository<Admin> adminRepository, IGenericRepository<Teacher> teacherRepository, IGenericRepository<Student> studentRepository)
    {
        _logger = logger;
        _userApiClient = userApiClient;
        _adminRepository = adminRepository;
        _teacherRepository = teacherRepository;
        _studentRepository = studentRepository;
    }

    public void DoWork(object state)
    {
        _logger.LogInformation("Database Sync Service is working.");
        SyncDatabase().Wait();
    }

    private async Task SyncDatabase()
    {
        _logger.LogInformation("Synchronizing with external database.");
        var students = await _userApiClient.FetchStudentProfiles();
        var teachers = await _userApiClient.FetchTeacherProfiles();
        foreach (var student in students)
        {
            var oldStudent = await _studentRepository.GetAsync(x => x.Email == student.Email);
            if (oldStudent == null)
            {
                continue;
            }
            else
            {
                oldStudent.Department = student.Department;
                oldStudent.Section = student.Section;
                oldStudent.Year = student.Year;

                await _studentRepository.UpdateAsync(oldStudent);
            }
            
        }

        foreach (var teacher in teachers)
        {
            var oldTeacher = await _teacherRepository.GetAsync(x => x.Email == teacher.Email);
            if (oldTeacher == null)
            {
                continue;
            }
            else
            {
                oldTeacher.Department = teacher.Department;
                await _teacherRepository.UpdateAsync(oldTeacher);
            }
        }
        
        
        
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Database Sync Service is stopping.");

        _timer?.Change(Timeout.Infinite, 0);

        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _timer?.Dispose();
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Database Sync Service is starting.");

        _timer = new Timer(DoWork, null, TimeSpan.Zero, 
            TimeSpan.FromMinutes(5));

        return Task.CompletedTask;
    }
}
