using MassTransit;
using AutoMapper;
using CustomEd.Contracts.Classroom.Events;
using CustomEd.Shared.Data.Interfaces;
using CustomEd.Forum.Service.Model;

namespace CustomEd.Forum.Service.Consumers
{
    public class ClassroomCreatedEventConsumer : IConsumer<ClassroomCreatedEvent>
    {
        private readonly IMapper _mapper;
        private readonly IGenericRepository<Model.Classroom> _classRoomRepository;
        private readonly IGenericRepository<Teacher> _teacherRepository;
        private readonly IGenericRepository<Student> _studentRepository;
        

        public ClassroomCreatedEventConsumer(IMapper mapper, IGenericRepository<Model.Classroom> classRoomRepository, IGenericRepository<Teacher> teacherRepository, IGenericRepository<Student> studentRepository)
        {
            _mapper = mapper;
            _classRoomRepository = classRoomRepository;
            _teacherRepository = teacherRepository;
            _studentRepository = studentRepository;
        }

        public async Task Consume(ConsumeContext<ClassroomCreatedEvent> context)
        {
            var classroom = _mapper.Map<Model.Classroom>(context.Message);
            classroom.Creator = await _teacherRepository.GetAsync(context.Message.CreatorId);
            classroom.Members = new List<Student>();
            foreach (var sid in context.Message.MemberIds)
            {
                var student = await _studentRepository.GetAsync(sid);
                classroom.Members.Add(_mapper.Map<Student>(student));
            }
            await _classRoomRepository.CreateAsync(classroom);
            
        }
    }
}
