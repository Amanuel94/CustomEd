using MassTransit;
using AutoMapper;
using CustomEd.Contracts.Classroom.Events;
using CustomEd.Shared.Data.Interfaces;

namespace CustomEd.RTNotification.Service.Consumers
{
    public class ClassroomDeletedEventConsumer : IConsumer<ClassroomDeletedEvent>
    {
        private readonly IMapper _mapper;
        private readonly IGenericRepository<Model.Classroom> _classRoomRepository;
        private readonly IGenericRepository<Model.Notification> _notificationRepository;

        public ClassroomDeletedEventConsumer(IMapper mapper, IGenericRepository<Model.Classroom> classRoomRepository, IGenericRepository<Model.Notification> notificationRepository)
        {
            _mapper = mapper;
            _classRoomRepository = classRoomRepository;
            _notificationRepository = notificationRepository;
        }

        public async Task Consume(ConsumeContext<ClassroomDeletedEvent> context)
        {
            var classroom = await _classRoomRepository.GetAsync(context.Message.Id);
            if (classroom == null)
            {
                return;
            }
            await _classRoomRepository.RemoveAsync(classroom);
            var notifications = await _notificationRepository.GetAllAsync(a => a.ClassroomId == classroom.Id);
            foreach (var notification in notifications)
            {
                await _notificationRepository.RemoveAsync(notification);
            }
            


        }
    }
}