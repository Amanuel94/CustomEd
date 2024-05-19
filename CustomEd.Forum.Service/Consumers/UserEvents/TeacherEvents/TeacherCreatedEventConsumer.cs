using MassTransit;
using CustomEd.Shared.Data.Interfaces;
using AutoMapper;
using CustomEd.User.Teacher.Events;
using CustomEd.Forum.Service.Model;

namespace CustomEd.Classroom.Service.Consumers
{
    public class TeacherCreatedEventConsumer : IConsumer<TeacherCreatedEvent>
    {
        private readonly IGenericRepository<Teacher> _teacherRepository;
        private readonly IMapper _mapper;

        public TeacherCreatedEventConsumer(IGenericRepository<Teacher> teacherRepository, IMapper mapper)
        {
    
            _teacherRepository = teacherRepository;
            _mapper = mapper;   
        }
        public async Task Consume(ConsumeContext<TeacherCreatedEvent> context)
        {
            Console.WriteLine("TeacherCreatedEventConsumer");
            var teacherCreatedEvent = context.Message;
            var teacher = _mapper.Map<Teacher>(teacherCreatedEvent);
            teacher.Id = teacherCreatedEvent.Id; 
            await _teacherRepository.CreateAsync(teacher);
            return;
        }
    }
}