using MassTransit;
using CustomEd.Shared.Data.Interfaces;
using CustomEd.Shared.Model;
using CustomEd.Contracts.Otp;

namespace CustomEd.User.Service.Consumers
{
    public class StudentCreatedEventConsumer : IConsumer<UserVerifiedEvent>
    {
        private readonly IGenericRepository<Model.Student> _studentRepository;
        private readonly IGenericRepository<Model.Teacher> _teacherRepository;

        public StudentCreatedEventConsumer(IGenericRepository<Model.Student> studentRepository, IGenericRepository<Model.Teacher> teacherRepository)
        {
            _studentRepository = studentRepository;
            _teacherRepository = teacherRepository;
        }

        public async Task Consume(ConsumeContext<UserVerifiedEvent> context)
        {

            var userVerifiedEvent = context.Message;
            if(userVerifiedEvent.Role == Role.Student)
            {
                var student = await _studentRepository.GetAsync(userVerifiedEvent.UserId);
                student.IsVerified = true;
                await _studentRepository.UpdateAsync(student);
            }
            else if(userVerifiedEvent.Role == Role.Teacher)
            {
                var teacher = await _teacherRepository.GetAsync(userVerifiedEvent.UserId);
                teacher.IsVerified = true;
                await _teacherRepository.UpdateAsync(teacher);
            }
        }
    }
}
