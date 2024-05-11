using System.Threading.Tasks;
using MassTransit;
using CustomEd.Shared.Data.Interfaces;
using CustomEd.User.Events;
using CustomEd.Shared.Model;
using CusotmEd.Contracts.User.Events;

namespace CustomEd.Otp.Service.Consumers
{
    public class UnverifiedUserEventConsumer : IConsumer<UnverifiedUserEvent>
    {
        private readonly IGenericRepository<Model.User> _userRepository;

        public UnverifiedUserEventConsumer(IGenericRepository<Model.User> userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task Consume(ConsumeContext<UnverifiedUserEvent> context)
        {
            Console.WriteLine("Log: Unverified User Event Consumed");
            Console.WriteLine($"Log:{context.Message.Id} {context.Message.Role}");
            var user = new Model.User { userId = context.Message.Id, Role = context.Message.Role };
            await _userRepository.CreateAsync(user);
            return;
        }
    }
}
