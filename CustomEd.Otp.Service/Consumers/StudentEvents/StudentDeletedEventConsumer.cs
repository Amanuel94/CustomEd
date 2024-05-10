// using System.Threading.Tasks;
// using MassTransit;
// using CustomEd.Shared.Data.Interfaces;
// using CustomEd.User.Student.Events;
// using CustomEd.Shared.Model;

// namespace CustomEd.Otp.Service.Consumers
// {
//     public class StudentDeletedEventConsumer : IConsumer<StudentDeletedEvent>
//     {
//         private readonly IGenericRepository<Model.User> _userRepository;

//         public StudentDeletedEventConsumer(IGenericRepository<Model.User> userRepository)
//         {
//             _userRepository = userRepository;
//         }

//         public async Task Consume(ConsumeContext<StudentDeletedEvent> context)
//         {
//             var StudentCreatedEvent = context.Message;
//             var user = new Model.User { userId = StudentCreatedEvent.Id, Role = Role.Student };
//             await _userRepository.RemoveAsync(user);
//             return;
//         }
//     }
// }
