using MassTransit;
using AutoMapper;
using CustomEd.Contracts.Classroom.Events;
using CustomEd.Shared.Data.Interfaces;

namespace CustomEd.RTNotification.Service.Consumers
{
    public class ClassroomCreatedEventConsumer : IConsumer<ClassroomCreatedEvent>
    {
        private readonly IMapper _mapper;
        private readonly IGenericRepository<Model.Classroom> _classRoomRepository;
        private readonly IGenericRepository<Model.User> _userRepository;

        public ClassroomCreatedEventConsumer(IMapper mapper, IGenericRepository<Model.Classroom> classRoomRepository, IGenericRepository<Model.User> userRepository)
        {
            _mapper = mapper;
            _classRoomRepository = classRoomRepository;
            _userRepository = userRepository;
        }

        public async Task Consume(ConsumeContext<ClassroomCreatedEvent> context)
        {
            var classroom = _mapper.Map<Model.Classroom>(context.Message);
            classroom.Creator  = await _userRepository.GetAsync(context.Message.CreatorId);
            await _classRoomRepository.CreateAsync(classroom);

        }
    }
}