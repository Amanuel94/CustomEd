using FluentValidation;
using CustomEd.User.Service.DTOs;
using CustomEd.User.Service.Model;
using CustomEd.Shared.Data.Interfaces;

namespace CustomEd.User.Service.Validators
{
    public class UpdateTeacherDtoValidator : AbstractValidator<UpdateTeacherDto>
    {
        private readonly IGenericRepository<Model.Teacher> _teacherRepository;
        public UpdateTeacherDtoValidator(IGenericRepository<Model.Teacher> teacherRepository)
        {
            _teacherRepository = teacherRepository;

            RuleFor(dto => dto.Id)
                .NotEmpty()
                .WithMessage("Teacher ID is required.")
                .MustAsync(async (id, cancellation) =>
                {
                    var teacher = await _teacherRepository.GetAsync(id);
                    return teacher != null;
                })
                .WithMessage("Teacher does not exist.");

            RuleFor(dto => dto.FirstName)
                .NotEmpty()
                .WithMessage("First name is required.")
                .MaximumLength(50)
                .WithMessage("First name must not exceed 50 characters.");

            RuleFor(dto => dto.LastName)
                .NotEmpty()
                .WithMessage("Last name is required.")
                .MaximumLength(50)
                .WithMessage("Last name must not exceed 50 characters.");

            RuleFor(dto => dto.DateOfBirth)
                .NotNull()
                .WithMessage("Date of birth is required.")
                .LessThan(DateOnly.Parse(DateTime.UtcNow.ToString("yyyy-MM-dd")))
                .WithMessage("Date of birth cannot be later than today.");

            RuleFor(dto => dto.Department)
                .NotEmpty()
                .WithMessage("Department is required.")
                .IsInEnum()
                .WithMessage("Invalid department.");

            RuleFor(dto => dto.PhoneNumber)
                .NotEmpty()
                .WithMessage("Phone number is required.")
                .MaximumLength(20)
                .WithMessage("Phone number must not exceed 20 digits.");

            RuleFor(dto => dto.JoinDate)
                .NotNull()
                .WithMessage("Join date is required.")
                .LessThan(DateOnly.Parse(DateTime.UtcNow.ToString("yyyy-MM-dd")))
                .WithMessage("Join date cannot be later than today.");

            // RuleFor(dto => dto.Email)
            //     .NotEmpty()
            //     .WithMessage("Email is required.")
            //     .MaximumLength(100)
            //     .WithMessage("Email must not exceed 100 characters.")
            //     .EmailAddress()
            //     .WithMessage("Invalid email format.");
            
            // RuleFor(dto => new { dto.Email, dto.Id })
            //     .MustAsync(async (data, cancellation) =>
            //     {
            //         var student = await _teacherRepository.GetAsync(s => s.Email == data.Email && s.Id != data.Id);
            //         return student == null;
            //     })
            //     .WithMessage("Email already exists.");
            
            // RuleFor(dto => dto.ImageUrl)
            //     .MaximumLength(200)
            //     .WithMessage("Image URL must not exceed 200 characters.");
                
            // RuleFor(dto => dto.Password)
            //     .NotEmpty()
            //     .WithMessage("Password is required.")
            //     .MinimumLength(8)
            //     .WithMessage("Password must be at least 8 characters long.");
            
        }
    }
}
