using MassTransit;
using AutoMapper;
using CustomEd.Contracts.Classroom.Events;
using CustomEd.Shared.Data.Interfaces;
using CustomEd.Forum.Service.Model;

namespace CustomEd.Forum.Service.Consumers
{
    public class MemberJoinedEventConsumer : IConsumer<MemberJoinedEvent>
    {
        private readonly IMapper _mapper;
        private readonly IGenericRepository<Student> _studentRepository;
        private readonly IGenericRepository<Model.Classroom> _classroomRepository;

        public MemberJoinedEventConsumer(IMapper mapper, IGenericRepository<Student> studentRepository, IGenericRepository<Model.Classroom> classroomRepository)
        {
            _mapper = mapper;
            _studentRepository = studentRepository;
            _classroomRepository = classroomRepository;
        }

        public async Task Consume(ConsumeContext<MemberJoinedEvent> context)
        {
            var classroom = await _classroomRepository.GetAsync(context.Message.ClassroomId);
            var student = await _studentRepository.GetAsync(context.Message.StudentId);
            if(classroom.Members == null){classroom.Members = new List<Student>();}
            classroom.Members.Add(student);
            Console.WriteLine("Here is the count of the members in the classroom:");
            Console.WriteLine(classroom.Members.Count);
            await _classroomRepository.UpdateAsync(classroom);

        }
    }
}
