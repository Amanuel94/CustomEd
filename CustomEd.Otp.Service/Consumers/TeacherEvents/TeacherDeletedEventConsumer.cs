// using System.Threading.Tasks;
// using MassTransit;
// using CustomEd.Shared.Data.Interfaces;
// using CustomEd.Shared.Model;
// using CustomEd.User.Contracts.Teacher.Events;

// namespace CustomEd.Otp.Service.Consumers
// {
//     public class TeacherDeletedEventConsumer : IConsumer<TeacherDeletedEvent>
//     {
//         private readonly IGenericRepository<Model.User> _userRepository;

//         public TeacherDeletedEventConsumer(IGenericRepository<Model.User> userRepository)
//         {
//             _userRepository = userRepository;
//         }

//         public async Task Consume(ConsumeContext<TeacherDeletedEvent> context)
//         {
//             var TeacherCreatedEvent = context.Message;
//             var user = new Model.User { userId = TeacherCreatedEvent.Id, Role = Role.Teacher };
//             await _userRepository.RemoveAsync(user);
//             return;
//         }
//     }
// }
