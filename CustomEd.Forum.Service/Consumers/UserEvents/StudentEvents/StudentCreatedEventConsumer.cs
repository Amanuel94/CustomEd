using System.Threading.Tasks;
using MassTransit;
using CustomEd.Shared.Data.Interfaces;
using AutoMapper;
using CustomEd.User.Student.Events;
using CustomEd.Forum.Service.Model;

namespace CustomEd.Forum.Service.Consumers
{
    public class StudentCreatedEventConsumer : IConsumer<StudentCreatedEvent>
    {
        private readonly IGenericRepository<Student> _StudentRepository;
        private readonly IMapper _mapper;

        public StudentCreatedEventConsumer(IGenericRepository<Student> StudentRepository, IMapper mapper)
        {
    
            _StudentRepository = StudentRepository;
            _mapper = mapper;   
        }
        public async Task Consume(ConsumeContext<StudentCreatedEvent> context)
        {
            var StudentCreatedEvent = context.Message;
            var Student = _mapper.Map<Student>(StudentCreatedEvent);
            Student.Id = StudentCreatedEvent.Id; 
            await _StudentRepository.CreateAsync(Student);
            return;
        }
    }
}
