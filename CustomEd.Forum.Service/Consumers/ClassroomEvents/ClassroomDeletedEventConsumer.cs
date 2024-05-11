using MassTransit;
using AutoMapper;
using CustomEd.Contracts.Classroom.Events;
using CustomEd.Shared.Data.Interfaces;
using CustomEd.Forum.Service.Model;

namespace CustomEd.Forum.Service.Consumers
{
    public class ClassroomDeletedEventConsumer : IConsumer<ClassroomDeletedEvent>
    {
        private readonly IMapper _mapper;
        private readonly IGenericRepository<Model.Classroom> _classRoomRepository;
        private readonly IGenericRepository<Service.Model.Message> _messageRepository;

        public ClassroomDeletedEventConsumer(IMapper mapper, IGenericRepository<Model.Classroom> classRoomRepository, IGenericRepository<Teacher> teacherRepository, IGenericRepository<Model.Message> messageRepository)
        {
            _mapper = mapper;
            _classRoomRepository = classRoomRepository;
            _messageRepository = messageRepository;
        }

        public async Task Consume(ConsumeContext<ClassroomDeletedEvent> context)
        {
            var classroom = await _classRoomRepository.GetAsync(context.Message.Id);
            if (classroom == null)
            {
                return;
            }
            await _classRoomRepository.RemoveAsync(classroom);
            var messages = await _messageRepository.GetAllAsync(x => x.Classroom!.Id == classroom.Id);
            foreach (var message in messages)
            {
                await _messageRepository.RemoveAsync(message);
            }

        }
    }
}
