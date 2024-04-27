using System.Threading.Tasks;
using MassTransit;
using CustomEd.User.Contracts;
using CustomEd.Shared.Data.Interfaces;
using AutoMapper;
using CustomEd.User.Student.Events;

namespace CustomEd.RTNotification.Service.Consumers
{
    public class StudentCreatedEventConsumer : IConsumer<StudentCreatedEvent>
    {
        private readonly IGenericRepository<Model.User> _userRepository;
        private readonly IMapper _mapper;

        public StudentCreatedEventConsumer(
            IGenericRepository<Model.User> userRepository,
            IMapper mapper
        )
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        
        public async Task Consume(ConsumeContext<StudentCreatedEvent> context)
        {
            var student = context.Message;
            var user = _mapper.Map<Model.User>(student);
            await _userRepository.CreateAsync(user);
            
        }
    }
}