using CustomEd.Shared.Data.Interfaces;
using CustomEd.User.Contracts.Teacher.Events;
using MassTransit;

namespace CustomEd.RTNotification.Service.Consumers
{
    public class TeacherDeletedEventConsumer : IConsumer<TeacherDeletedEvent>
    {
        private readonly IGenericRepository<Model.User> _teacherRepository;
        private readonly IGenericRepository<Model.Classroom> _classroomRepository;
        private readonly IGenericRepository<Model.Notification> _notificationRepository;

        public TeacherDeletedEventConsumer(IGenericRepository<Model.User> teacherRepository, IGenericRepository<Model.Classroom> classroomRepository, IGenericRepository<Model.Notification> notificationRepository)
        {
            _teacherRepository = teacherRepository;
            _classroomRepository = classroomRepository;
            _notificationRepository = notificationRepository;
        }


        public async Task Consume(ConsumeContext<TeacherDeletedEvent> context)
        {
            var teacherDeletedEvent = context.Message;
            await _teacherRepository.RemoveAsync(teacherDeletedEvent.Id);
            var classrooms = await _classroomRepository.GetAllAsync(x =>
                x.Creator.Id == teacherDeletedEvent.Id
            );
            foreach (var classroom in classrooms)
            {
                await _classroomRepository.RemoveAsync(classroom.Id);
                var notifications = await _notificationRepository.GetAllAsync(a =>
                    a.ClassroomId == classroom.Id
                );
                foreach (var notification in notifications)
                {
                    await _notificationRepository.RemoveAsync(notification);
                }
            }

            return;
        }
    }
}
