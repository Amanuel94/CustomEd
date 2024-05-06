using MassTransit;
using AutoMapper;
using CustomEd.Contracts.Classroom.Events;
using CustomEd.Announcement.Service.Model;
using CustomEd.Shared.Data.Interfaces;

namespace CustomEd.Announcement.Service.Consumers
{
    public class MemberLeftEventConsumer : IConsumer<MemberLeftEvent>
    {
        private readonly IMapper _mapper;
        private readonly IGenericRepository<ClassRoom> _classRoomRepository;

        public MemberLeftEventConsumer(IMapper mapper, IGenericRepository<ClassRoom> classRoomRepository)
        {
            _mapper = mapper;
            _classRoomRepository = classRoomRepository;
        }

        public async Task Consume(ConsumeContext<MemberLeftEvent> context)
        {
            var classroom = await _classRoomRepository.GetAsync(context.Message.ClassroomId);
            Console.WriteLine(classroom.MemberIds.Count);
            classroom.MemberIds = classroom.MemberIds.Where(x => x != context.Message.StudentId).ToList();
            await _classRoomRepository.UpdateAsync(classroom);

        }
    }
}