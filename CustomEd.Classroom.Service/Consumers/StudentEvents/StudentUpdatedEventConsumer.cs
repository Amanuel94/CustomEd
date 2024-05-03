using System.Threading.Tasks;
using MassTransit;
using CustomEd.Shared.Data.Interfaces;
using CustomEd.Classroom.Service.Model;
using AutoMapper;
using CustomEd.User.Student.Events;

namespace CustomEd.Classroom.Service.Consumers
{
    public class StudentUpdatedEventConsumer : IConsumer<StudentUpdatedEvent>
    {
        private readonly IGenericRepository<Student> _studentRepository;
        private readonly IGenericRepository<Model.Classroom> _classroomRepository;
        private readonly IMapper _mapper;

        public StudentUpdatedEventConsumer(IGenericRepository<Student> StudentRepository, IGenericRepository<Model.Classroom> classroomRepository, IMapper mapper)
        {
            _studentRepository = StudentRepository;
            _classroomRepository = classroomRepository;
            _mapper = mapper;   
        }
        public async Task Consume(ConsumeContext<StudentUpdatedEvent> context)
        {
            var StudentUpdatedEvent = context.Message;
            var Student = _mapper.Map<Student>(StudentUpdatedEvent);
            Student.Id = StudentUpdatedEvent.Id; 
            await _studentRepository.UpdateAsync(Student);
            var student = await _studentRepository.GetAsync(StudentUpdatedEvent.Id);
            var classrooms = await _classroomRepository.GetAllAsync(x => x.Members.Contains(student));
            foreach (var classroom in classrooms)
            {
                classroom.Members = classroom.Members.Where(x => x.Id != StudentUpdatedEvent.Id).ToList();
                classroom.Members.Add(student);
                await _classroomRepository.UpdateAsync(classroom);
            }

            return;
        }
    }
}