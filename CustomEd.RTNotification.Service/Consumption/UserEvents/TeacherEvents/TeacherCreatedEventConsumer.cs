using MassTransit;
using CustomEd.Shared.Data.Interfaces;
using AutoMapper;
using CustomEd.User.Teacher.Events;

namespace CustomEd.RTNotification.Service.Consumers
{
    public class TeacherCreatedEventConsumer : IConsumer<TeacherCreatedEvent>
    {
        private readonly IGenericRepository<Model.User> _teacherRepository;
        private readonly IMapper _mapper;

        public TeacherCreatedEventConsumer(IGenericRepository<Model.User> teacherRepository, IMapper mapper)
        {
    
            _teacherRepository = teacherRepository;
            _mapper = mapper;   
        }
        public async Task Consume(ConsumeContext<TeacherCreatedEvent> context)
        {
            var teacherCreatedEvent = context.Message;
            var teacher = _mapper.Map<Model.User>(teacherCreatedEvent);
            teacher.Id = teacherCreatedEvent.Id; 
            await _teacherRepository.CreateAsync(teacher);
            return;
        }
    }
}