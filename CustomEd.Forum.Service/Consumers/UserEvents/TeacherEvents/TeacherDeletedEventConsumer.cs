using MassTransit;
using CustomEd.Shared.Data.Interfaces;
using CustomEd.User.Contracts.Teacher.Events;
using CustomEd.Contracts.Classroom.Events;
using CustomEd.Forum.Service.Model;

namespace CustomEd.Forum.Service.Consumers
{
    public class TeacherDeletedEventConsumer : IConsumer<TeacherDeletedEvent>
    {
        private readonly IGenericRepository<Teacher> _teacherRepository;
        private readonly IGenericRepository<Model.Classroom> _classroomRepository;
        private readonly IPublishEndpoint _publishEndpoint;

        public TeacherDeletedEventConsumer(IGenericRepository<Teacher> teacherRepository, IGenericRepository<Model.Classroom> classroomRepository, IPublishEndpoint publishEndpoint)
        {
            _teacherRepository = teacherRepository;
            _classroomRepository = classroomRepository;
            _publishEndpoint = publishEndpoint;
        }
        public async Task Consume(ConsumeContext<TeacherDeletedEvent> context)
        {
            var teacherDeletedEvent = context.Message; 
            await _teacherRepository.RemoveAsync(teacherDeletedEvent.Id);
            var classrooms = await _classroomRepository.GetAllAsync(x => x.Creator.Id == teacherDeletedEvent.Id);
            foreach (var classroom in classrooms)
            {
                await _classroomRepository.RemoveAsync(classroom.Id);
            }
            
            return;
        }
    }
}
