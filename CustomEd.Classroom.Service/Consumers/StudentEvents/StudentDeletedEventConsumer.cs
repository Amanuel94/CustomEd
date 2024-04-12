using System.Threading.Tasks;
using MassTransit;
using CustomEd.Shared.Data.Interfaces;
using CustomEd.Classroom.Service.Model;
using AutoMapper;
using CustomEd.User.Student.Events;

namespace CustomEd.Classroom.Service.Consumers
{
    public class StudentDeletedEventConsumer : IConsumer<StudentDeletedEvent>
    {
        private readonly IGenericRepository<Student> _studentRepository;
        private readonly IGenericRepository<Model.Classroom> _classroomRepository;
        private readonly IMapper _mapper;

        public StudentDeletedEventConsumer(IGenericRepository<Student> StudentRepository, IGenericRepository<Model.Classroom> classroomRepository, IMapper mapper)
        {
    
            _studentRepository = StudentRepository;
            _classroomRepository = classroomRepository;
            _mapper = mapper;   
        }
        public async Task Consume(ConsumeContext<StudentDeletedEvent> context)
        {
            var StudentDeletedEvent = context.Message;
            var student = await _studentRepository.GetAsync(StudentDeletedEvent.Id);
            await _studentRepository.RemoveAsync(StudentDeletedEvent.Id);

            var classrooms = await _classroomRepository.GetAllAsync(x => x.Members.Contains(student));
            foreach (var classroom in classrooms)
            {
                classroom.Members.Remove(student);
                await _classroomRepository.UpdateAsync(classroom);
            }
            return;
        }
    }
}