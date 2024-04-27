using MassTransit;
using AutoMapper;
using CustomEd.Contracts.Classroom.Events;
using CustomEd.Shared.Data.Interfaces;

namespace CustomEd.RTNotification.Service.Consumers
{
    public class ClassroomUpdatedEventConsumer : IConsumer<ClassroomUpdatedEvent>
    {
        private readonly IMapper _mapper;
        private readonly IGenericRepository<Model.Classroom> _classRoomRepository;

        public ClassroomUpdatedEventConsumer(IMapper mapper, IGenericRepository<Model.Classroom> classRoomRepository)
        {
            _mapper = mapper;
            _classRoomRepository = classRoomRepository;
        }

        public async Task Consume(ConsumeContext<ClassroomUpdatedEvent> context)
        {
            var classroom = _mapper.Map<Model.Classroom>(context.Message);
            await _classRoomRepository.UpdateAsync(classroom);
            
        }
    }
}