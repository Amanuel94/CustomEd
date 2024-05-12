using MassTransit;
using AutoMapper;
using CustomEd.Contracts.Classroom.Events;
using CustomEd.Shared.Data.Interfaces;
using CustomEd.RTNotification.Service.Model;

namespace CustomEd.RTNotification.Service.Consumers
{
    public class MemberLeftEventConsumer : IConsumer<MemberLeftEvent>
    {
        private readonly IMapper _mapper;
        private readonly IGenericRepository<Model.Classroom> _classRoomRepository;
        private readonly IGenericRepository<Model.User> _userRepository;

        public MemberLeftEventConsumer(IMapper mapper, IGenericRepository<Classroom> classRoomRepository, IGenericRepository<Model.User> userRepository)
        {
            _mapper = mapper;
            _classRoomRepository = classRoomRepository;
            _userRepository = userRepository;
        }

        public async Task Consume(ConsumeContext<MemberLeftEvent> context)
        {
            var clasroom = await _classRoomRepository.GetAsync(context.Message.ClassroomId);
            var classroom = _mapper.Map<Classroom>(clasroom);

            var user = await _userRepository.GetAsync(context.Message.StudentId);
            if (user == null)
            {
                return;
            }
            classroom.Members = classroom.Members.Where(a => a.Id != user.Id).ToList();
            await _classRoomRepository.UpdateAsync(classroom);
        }
    }
}