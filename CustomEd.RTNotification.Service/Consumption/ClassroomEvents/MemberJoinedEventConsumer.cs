using MassTransit;
using AutoMapper;
using CustomEd.Contracts.Classroom.Events;
using CustomEd.Shared.Data.Interfaces;
using CustomEd.RTNotification.Service.Model;

namespace CustomEd.RTNotification.Service.Consumers
{
    public class MemberJoinedEventConsumer : IConsumer<MemberJoinedEvent>
    {
        private readonly IMapper _mapper;
        private readonly IGenericRepository<Model.Classroom> _classRoomRepository;
        private readonly IGenericRepository<Model.User> _userRepository;

        public MemberJoinedEventConsumer(IMapper mapper, IGenericRepository<Classroom> classRoomRepository, IGenericRepository<Model.User> userRepository)
        {
            _mapper = mapper;
            _classRoomRepository = classRoomRepository;
            _userRepository = userRepository;
        }

        public async Task Consume(ConsumeContext<MemberJoinedEvent> context)
        {
            var clasroom = await _classRoomRepository.GetAsync(context.Message.ClassroomId);
            var classroom = _mapper.Map<Classroom>(clasroom);

            var user = await _userRepository.GetAsync(context.Message.StudentId);
            if (user == null)
            {
                return;
            }
            classroom.Members.Add(user);
            await _classRoomRepository.UpdateAsync(classroom);
        }
    }
}