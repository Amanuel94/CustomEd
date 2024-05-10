using MassTransit;
using CustomEd.Shared.Data.Interfaces;
using CustomEd.Shared.Model;
using CustomEd.Contracts.Otp;
using AutoMapper;
using CustomEd.User.Student.Events;
using CustomEd.User.Teacher.Events;

namespace CustomEd.User.Service.Consumers
{
    public class UserVerifiedEventConsumer : IConsumer<UserVerifiedEvent>
    {
        private readonly IGenericRepository<Model.Student> _studentRepository;
        private readonly IGenericRepository<Model.Teacher> _teacherRepository;
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly IMapper _mapper;

        public UserVerifiedEventConsumer(IGenericRepository<Model.Student> studentRepository, IGenericRepository<Model.Teacher> teacherRepository, IPublishEndpoint publishEndpoint, IMapper mapper)
        {
            _studentRepository = studentRepository;
            _teacherRepository = teacherRepository;
            _publishEndpoint = publishEndpoint;
            _mapper = mapper;
        }

        public async Task Consume(ConsumeContext<UserVerifiedEvent> context)
        {

            var userVerifiedEvent = context.Message;
            if(userVerifiedEvent.Role == Role.Student)
            {
                var student = await _studentRepository.GetAsync(userVerifiedEvent.UserId);
                student.IsVerified = true;
                await _studentRepository.UpdateAsync(student);
                var studentCreatedEvent = _mapper.Map<StudentCreatedEvent>(student);
                Console.WriteLine("Log: Student Published");
                await _publishEndpoint.Publish(studentCreatedEvent);
             
                
            }
            else if(userVerifiedEvent.Role == Role.Teacher)
            {
                var teacher = await _teacherRepository.GetAsync(userVerifiedEvent.UserId);
                teacher.IsVerified = true;
                await _teacherRepository.UpdateAsync(teacher);
                var teacherCreatedEvent = _mapper.Map<TeacherCreatedEvent>(teacher);
                Console.WriteLine("Log: Teacher Published");
                await _publishEndpoint.Publish(teacherCreatedEvent);
            }
        }
    }
}
