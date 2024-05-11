using MassTransit;
using AutoMapper;
using CustomEd.Shared.Data.Interfaces;
using CustomEd.Contracts.Classroom.Events;
using CustomEd.Forum.Service.Model;

namespace CustomEd.Forum.Service.Consumers
{
    public class MemberLeftEventConsumer : IConsumer<MemberLeftEvent>
    {
        private readonly IMapper _mapper;
        private readonly IGenericRepository<Model.Classroom> _classRoomRepository;

        public MemberLeftEventConsumer(IMapper mapper, IGenericRepository<Model.Classroom> classRoomRepository)
        {
            _mapper = mapper;
            _classRoomRepository = classRoomRepository;
        }

        public async Task Consume(ConsumeContext<MemberLeftEvent> context)
        {
            var classroom = await _classRoomRepository.GetAsync(context.Message.ClassroomId);
            Console.WriteLine(classroom.Members.Count);
            classroom.Members = classroom.Members.Where(x => x.Id != context.Message.StudentId).ToList();
            await _classRoomRepository.UpdateAsync(classroom);

        }
    }
}
