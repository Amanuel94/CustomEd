// using System.Threading.Tasks;
// using MassTransit;
// using CustomEd.Shared.Data.Interfaces;

// using CustomEd.User.Teacher.Events;
// using CustomEd.Shared.Model;

// namespace CustomEd.Otp.Service.Consumers
// {
//     public class TeacherCreatedEventConsumer : IConsumer<TeacherCreatedEvent>
//     {
//         private readonly IGenericRepository<Model.User> _userRepository;

//         public TeacherCreatedEventConsumer(IGenericRepository<Model.User> userRepository)
//         {
//             _userRepository = userRepository;
//         }

//         public async Task Consume(ConsumeContext<TeacherCreatedEvent> context)
//         {

//             var TeacherCreatedEvent = context.Message;
//             var user = new Model.User { userId = TeacherCreatedEvent.Id, Role = Role.Teacher };
//             await _userRepository.CreateAsync(user);
//             return;
//         }
//     }
// }
