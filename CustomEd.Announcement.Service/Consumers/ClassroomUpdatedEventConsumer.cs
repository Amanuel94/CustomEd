using MassTransit;
using AutoMapper;
using CustomEd.Contracts.Classroom.Events;
using CustomEd.Announcement.Service.Model;
using CustomEd.Shared.Data.Interfaces;

namespace CustomEd.Announcement.Service.Consumers
{
    public class ClassroomUpdatedEventConsumer : IConsumer<ClassroomUpdatedEvent>
    {
        private readonly IMapper _mapper;
        private readonly IGenericRepository<ClassRoom> _classRoomRepository;
        private readonly IGenericRepository<Teacher> _teacherRepository;
        private readonly IGenericRepository<Model.Announcement> _announcementRepository;

        public ClassroomUpdatedEventConsumer(IMapper mapper, IGenericRepository<ClassRoom> classRoomRepository, IGenericRepository<Teacher> teacherRepository, IGenericRepository<Model.Announcement> announcementRepository)
        {
            _mapper = mapper;
            _classRoomRepository = classRoomRepository;
            _teacherRepository = teacherRepository;
            _announcementRepository = announcementRepository;
        }

        public async Task Consume(ConsumeContext<ClassroomUpdatedEvent> context)
        {
            var classroom = _mapper.Map<ClassRoom>(context.Message);
            var newList = new List<Guid>();
            foreach (var sid in context.Message.MemberIds)
            {
                newList.Add(sid);
            }
            classroom.MemberIds = newList;
            await _classRoomRepository.UpdateAsync(classroom);
            var announcements = await _announcementRepository.GetAllAsync(x => x.ClassRoom.Id == classroom.Id);
            foreach (var announcement in announcements)
            {
                await _announcementRepository.UpdateAsync(announcement);
            }
        }
    }
}