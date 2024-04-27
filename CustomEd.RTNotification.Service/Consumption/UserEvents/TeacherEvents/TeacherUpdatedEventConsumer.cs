using System.Threading.Tasks;
using MassTransit;
using CustomEd.Shared.Data.Interfaces;
using AutoMapper;
using CustomEd.User.Teacher.Events;

namespace CustomEd.RTNotification.Service.Consumers
{
    public class TeacherUpdatedEventConsumer : IConsumer<TeacherUpdatedEvent>
    {
        private readonly IGenericRepository<Model.User> _teacherRepository;
        private readonly IGenericRepository<Model.Classroom> _classroomRepository;
        private readonly IMapper _mapper;

        public TeacherUpdatedEventConsumer(IGenericRepository<Model.User> teacherRepository, IGenericRepository<Model.Classroom> classroomRepository, IMapper mapper)
        {
            _teacherRepository = teacherRepository;
            _classroomRepository = classroomRepository;
            _mapper = mapper;
        }

        public async Task Consume(ConsumeContext<TeacherUpdatedEvent> context)
        {
            var teacherUpdatedEvent = context.Message;
            var teacher = _mapper.Map<Model.User>(teacherUpdatedEvent);
            teacher.Id = teacherUpdatedEvent.Id; 
            await _teacherRepository.UpdateAsync(teacher);
            var classrooms = await _classroomRepository.GetAllAsync(x => x.Creator.Id == teacherUpdatedEvent.Id);
            foreach (var classroom in classrooms)
            {
                classroom.Creator = teacher;
                await _classroomRepository.UpdateAsync(classroom);
            } 
            return;
        }
        
    }
}
