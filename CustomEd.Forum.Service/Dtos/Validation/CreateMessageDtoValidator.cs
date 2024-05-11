using FluentValidation;
using CustomEd.Forum.Service.Dto;
using CustomEd.Shared.Data.Interfaces;
using CustomEd.Forum.Service.Model;

namespace CustomEd.Forum.Service.Dto;
public class CreateMessageDtoValidator<T> : AbstractValidator<CreateMessageDto> where T:Model.User
{
    private readonly IGenericRepository<T> _userRepository;
    private readonly IGenericRepository<Model.Classroom> _classroomRepository;
    private readonly IGenericRepository<Message> _messageRepository;
    public CreateMessageDtoValidator(IGenericRepository<T> userRepository, IGenericRepository<Model.Classroom> classroomRepository, IGenericRepository<Message> messageRepository)
    {
        _userRepository = userRepository;
        _classroomRepository = classroomRepository;
        _messageRepository = messageRepository;

        RuleFor(x => x.Content)
            .NotEmpty().WithMessage("Content is required.")
            .Length(1, 500).WithMessage("Content must be between 1 and 500 characters.");

        RuleFor(x => x.SenderId)
            .NotEmpty().WithMessage("SenderId is required.")
            .MustAsync(async (senderId, cancellationToken) =>
            {
                var sender = await _userRepository.GetAsync(senderId);
                return sender != null;
            }).WithMessage("Sender does not exist.");
    

        RuleFor(x => x.ClassroomId)
            .NotEmpty().WithMessage("ClassroomId is required.")
            .MustAsync(async (classroomId, cancellationToken) =>
            {
                var classroom = await _classroomRepository.GetAsync(classroomId);
                return classroom != null;
            }).WithMessage("Classroom does not exist.");
    }
}
