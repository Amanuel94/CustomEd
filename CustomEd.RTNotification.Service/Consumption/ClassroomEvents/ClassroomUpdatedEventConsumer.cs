using MassTransit;
using AutoMapper;
using CustomEd.Contracts.Classroom.Events;
using CustomEd.Shared.Data.Interfaces;
using CustomEd.RTNotification.Service.Model;

namespace CustomEd.RTNotification.Service.Consumers
{
    public class ClassroomUpdatedEventConsumer : IConsumer<ClassroomUpdatedEvent>
    {
        private readonly IMapper _mapper;
        private readonly IGenericRepository<Model.Classroom> _classRoomRepository;
        private readonly IGenericRepository<Model.User> _userRepository;

        public ClassroomUpdatedEventConsumer(IMapper mapper, IGenericRepository<Classroom> classRoomRepository, IGenericRepository<Model.User> userRepository)
        {
            _mapper = mapper;
            _classRoomRepository = classRoomRepository;
            _userRepository = userRepository;
        }

        public async Task Consume(ConsumeContext<ClassroomUpdatedEvent> context)
        {
            var classroom = _mapper.Map<Model.Classroom>(context.Message);
            classroom.Creator = await _userRepository.GetAsync(context.Message.CreatorId);
            var newList = new List<Model.User>();
            foreach (var sid in context.Message.MemberIds)
            {
                var user = await _userRepository.GetAsync(sid);
                newList.Add(user);
            }
            classroom.Members = newList;
            await _classRoomRepository.UpdateAsync(classroom);
            
        }
    }
}