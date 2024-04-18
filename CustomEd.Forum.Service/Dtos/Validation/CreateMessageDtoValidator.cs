using FluentValidation;
using CustomEd.Forum.Service.Dto;
using CustomEd.Shared.Data.Interfaces;
using CustomEd.Forum.Service.Model;

public class CreateMessageDtoValidator : AbstractValidator<CreateMessageDto>
{
    private readonly IGenericRepository<User> _userRepository;
    private readonly IGenericRepository<Classroom> _classroomRepository;
    public CreateMessageDtoValidator(IGenericRepository<User> userRepository, IGenericRepository<Classroom> classroomRepository)
    {
        _userRepository = userRepository;
        _classroomRepository = classroomRepository;

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
