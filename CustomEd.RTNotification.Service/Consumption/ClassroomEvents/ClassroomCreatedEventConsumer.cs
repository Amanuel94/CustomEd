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

        public ClassroomCreatedEventConsumer(IMapper mapper, IGenericRepository<Model.Classroom> classRoomRepository)
        {
            _mapper = mapper;
            _classRoomRepository = classRoomRepository;
        }

        public async Task Consume(ConsumeContext<ClassroomCreatedEvent> context)
        {
            var classroom = _mapper.Map<Model.Classroom>(context.Message);
            await _classRoomRepository.CreateAsync(classroom);

        }
    }
}