using System.Threading.Tasks;
using MassTransit;
using CustomEd.Shared.Data.Interfaces;
using AutoMapper;
using CustomEd.User.Student.Events;

namespace CustomEd.RTNotification.Service.Consumers
{
    public class StudentUpdatedEventConsumer : IConsumer<StudentUpdatedEvent>
    {
        private readonly IGenericRepository<Model.User> _userRepository;
        private readonly IGenericRepository<Model.Classroom> _classroomRepository;
        private readonly IMapper _mapper;

        public StudentUpdatedEventConsumer(
            IGenericRepository<Model.User> userRepository,
            IGenericRepository<Model.Classroom> classroomRepository,
            IMapper mapper
        )
        {
            _userRepository = userRepository;
            _classroomRepository = classroomRepository;
            _mapper = mapper;
        }
        public async Task Consume(ConsumeContext<StudentUpdatedEvent> context)
        {
            var student = context.Message;
            var user = await _userRepository.GetAsync(student.Id);
            if (user == null)
            {
                return;
            }
            _mapper.Map(student, user);
            await _userRepository.UpdateAsync(user);
        }
    }
}