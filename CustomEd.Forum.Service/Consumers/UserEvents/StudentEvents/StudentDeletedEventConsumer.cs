using System.Threading.Tasks;
using MassTransit;
using CustomEd.Shared.Data.Interfaces;
using AutoMapper;
using CustomEd.User.Student.Events;
using CustomEd.Contracts.Classroom.Events;
using CustomEd.Forum.Service.Model;

namespace CustomEd.Forum.Service.Consumers
{
    public class StudentDeletedEventConsumer : IConsumer<StudentDeletedEvent>
    {
        private readonly IGenericRepository<Student> _studentRepository;
        private readonly IGenericRepository<Model.Classroom> _classroomRepository;
        private readonly IMapper _mapper;
        private readonly IPublishEndpoint _publishEndpoint;

        public StudentDeletedEventConsumer(IGenericRepository<Student> StudentRepository, IGenericRepository<Model.Classroom> classroomRepository, IMapper mapper, IPublishEndpoint publishEndpoint)
        {
    
            _studentRepository = StudentRepository;
            _classroomRepository = classroomRepository;
            _mapper = mapper;   
            _publishEndpoint = publishEndpoint;
        }
        public async Task Consume(ConsumeContext<StudentDeletedEvent> context)
        {
            var StudentDeletedEvent = context.Message;
            var student = await _studentRepository.GetAsync(StudentDeletedEvent.Id);
            var classrooms = await _classroomRepository.GetAllAsync(x => x.Members.Select(x => x.Id).Contains(StudentDeletedEvent.Id));
            foreach (var classroom in classrooms)
            {
                classroom.Members = classroom.Members.Where(x => x.Id != StudentDeletedEvent.Id).ToList();
                await _classroomRepository.UpdateAsync(classroom);
            }
            await _studentRepository.RemoveAsync(StudentDeletedEvent.Id);
            return;
        }
    }
}
