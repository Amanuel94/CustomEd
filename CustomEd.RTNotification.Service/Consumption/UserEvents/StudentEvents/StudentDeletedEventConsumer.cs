using System.Threading.Tasks;
using MassTransit;
using CustomEd.Shared.Data.Interfaces;
using AutoMapper;
using CustomEd.User.Student.Events;

namespace CustomEd.RTNotification.Service.Consumers
{
    public class StudentDeletedEventConsumer : IConsumer<StudentDeletedEvent>
    {
        private readonly IGenericRepository<Model.User> _studentRepository;
        private readonly IGenericRepository<Model.Classroom> _classroomRepository;
        private readonly IMapper _mapper;

        public StudentDeletedEventConsumer(
            IGenericRepository<Model.User> studentRepository,
            IGenericRepository<Model.Classroom> classroomRepository,
            IMapper mapper
        )
        {
            _studentRepository = studentRepository;
            _classroomRepository = classroomRepository;
            _mapper = mapper;
        }
        public async Task Consume(ConsumeContext<StudentDeletedEvent> context)
        {
            var student = context.Message;
            var user = await _studentRepository.GetAsync(student.Id);
            if (user == null)
            {
                return;
            }
            await _studentRepository.RemoveAsync(user);

            var classrooms = await _classroomRepository.GetAllAsync(x => x.Members.Select(m => m.Id).Contains(student.Id));
            foreach (var classroom in classrooms)
            {
                classroom.Members.Remove(user);
                await _classroomRepository.UpdateAsync(classroom);
            }
        }
    }
}