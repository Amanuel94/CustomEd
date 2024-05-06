using MassTransit;
using AutoMapper;
using CustomEd.Contracts.Classroom.Events;
using CustomEd.Announcement.Service.Model;
using CustomEd.Shared.Data.Interfaces;

namespace CustomEd.Announcement.Service.Consumers
{
    public class ClassroomDeletedEventConsumer : IConsumer<ClassroomDeletedEvent>
    {
        private readonly IMapper _mapper;
        private readonly IGenericRepository<ClassRoom> _classRoomRepository;
        private readonly IGenericRepository<Teacher> _teacherRepository;
        private readonly IGenericRepository<Model.Announcement> _announcementRepository;

        public ClassroomDeletedEventConsumer(IMapper mapper, IGenericRepository<ClassRoom> classRoomRepository, IGenericRepository<Teacher> teacherRepository, IGenericRepository<Model.Announcement> announcementRepository)
        {
            _mapper = mapper;
            _classRoomRepository = classRoomRepository;
            _teacherRepository = teacherRepository;
            _announcementRepository = announcementRepository;
        }

        public async Task Consume(ConsumeContext<ClassroomDeletedEvent> context)
        {
            var classroom = await _classRoomRepository.GetAsync(context.Message.Id);
            if (classroom == null)
            {
                return;
            }
            await _classRoomRepository.RemoveAsync(classroom);
            var announcements = await _announcementRepository.GetAllAsync(x => x.ClassRoom.Id == classroom.Id);
            foreach (var announcement in announcements)
            {
                await _announcementRepository.RemoveAsync(announcement);
            }

        }
    }
}