using MassTransit;
using AutoMapper;
using CustomEd.Contracts.Classroom.Events;
using CustomEd.Announcement.Service.Model;
using CustomEd.Shared.Data.Interfaces;

namespace CustomEd.Announcement.Service.Consumers
{
    public class MemberJoinedEventConsumer : IConsumer<MemberJoinedEvent>
    {
        private readonly IMapper _mapper;
        private readonly IGenericRepository<ClassRoom> _classRoomRepository;

        public MemberJoinedEventConsumer(IMapper mapper, IGenericRepository<ClassRoom> classRoomRepository)
        {
            _mapper = mapper;
            _classRoomRepository = classRoomRepository;
        }

        public async Task Consume(ConsumeContext<MemberJoinedEvent> context)
        {
            var classroom = await _classRoomRepository.GetAsync(context.Message.ClassroomId);
            if(classroom.MemberIds == null)
            {
                classroom.MemberIds = new List<Guid>();
            }
            classroom.MemberIds.Add(context.Message.StudentId);
            Console.WriteLine("Here is the count of the members in the classroom:");
            Console.WriteLine(classroom.MemberIds.Count);
            await _classRoomRepository.UpdateAsync(classroom);

        }
    }
}