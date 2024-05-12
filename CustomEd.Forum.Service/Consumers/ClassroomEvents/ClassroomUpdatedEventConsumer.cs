using MassTransit;
using AutoMapper;
using CustomEd.Contracts.Classroom.Events;
using CustomEd.Shared.Data.Interfaces;
using CustomEd.Forum.Service.Model;

namespace CustomEd.Forum.Service.Consumers
{
    public class ClassroomUpdatedEventConsumer : IConsumer<ClassroomUpdatedEvent>
    {
        private readonly IMapper _mapper;
        private readonly IGenericRepository<Model.Classroom> _classRoomRepository;
        private readonly IGenericRepository<Teacher> _teacherRepository;
        private readonly IGenericRepository<Student> _studentRepository;        
        private readonly IGenericRepository<Message> _messageRepository;

        public ClassroomUpdatedEventConsumer(IMapper mapper, IGenericRepository<Model.Classroom> classRoomRepository, IGenericRepository<Message> messageRepository, IGenericRepository<Student> studentRepository,  IGenericRepository<Teacher> teacherRepository)
        {
            _mapper = mapper;
            _classRoomRepository = classRoomRepository;
            _studentRepository = studentRepository;
            _teacherRepository = teacherRepository;
            _messageRepository = messageRepository;
            
        }

        public async Task Consume(ConsumeContext<ClassroomUpdatedEvent> context)
        {
            var classroom = _mapper.Map<Model.Classroom>(context.Message);
            classroom.Creator = await _teacherRepository.GetAsync(context.Message.CreatorId);
            var newList = new List<Student>();
            foreach (var sid in context.Message.MemberIds)
            {
                Console.WriteLine(sid);
                var student = await _studentRepository.GetAsync(sid);
                newList.Add(student);
            }
        
            classroom.Members = newList;
            await _classRoomRepository.UpdateAsync(classroom);
            var messages = await _messageRepository.GetAllAsync(x => x.Classroom!.Id == classroom.Id);

            foreach (var message in messages)
            {
                await _messageRepository.UpdateAsync(message);
            }

            // var classroom = _mapper.Map<Model.Classroom>(context.Message);
            // classroom.Creator = await _teacherRepository.GetAsync(context.Message.CreatorId);
            // classroom.Members = new List<Student>();
            // foreach (var sid in context.Message.MemberIds)
            // {
            //     var student = await _studentRepository.GetAsync(sid);
            //     classroom.Members.Add(_mapper.Map<Student>(student));
            // }
            // await _classRoomRepository.CreateAsync(classroom);
        }
    }
}
