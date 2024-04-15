using System.Threading.Tasks;
using MassTransit;
using CustomEd.Shared.Data.Interfaces;

using CustomEd.User.Student.Events;
using CustomEd.Shared.Model;

namespace CustomEd.Otp.Service.Consumers
{
    public class StudentCreatedEventConsumer : IConsumer<StudentCreatedEvent>
    {
        private readonly IGenericRepository<Model.User> _userRepository;

        public StudentCreatedEventConsumer(IGenericRepository<Model.User> userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task Consume(ConsumeContext<StudentCreatedEvent> context)
        {

            var StudentCreatedEvent = context.Message;
            var user = new Model.User { userId = StudentCreatedEvent.Id, Role = Role.Student };
            await _userRepository.CreateAsync(user);
            return;
        }
    }
}
