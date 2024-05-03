using CustomEd.Shared.Data.Interfaces;
using FluentValidation;
using System;

namespace CustomEd.Classroom.Service.DTOs.Validation
{
    public class RemoveStudentDtoValidator : AbstractValidator<RemoveStudentDto>
    {
        private IGenericRepository<Model.Student> _studentRepository;
        private IGenericRepository<Model.Classroom> _classroomRepository;
        public RemoveStudentDtoValidator(IGenericRepository<Model.Student> studentRepository, IGenericRepository<Model.Classroom> classroomRepository)
        {
            _studentRepository = studentRepository;
            _classroomRepository = classroomRepository;
    
            RuleFor(x => x.StudentId)
                .NotEmpty().WithMessage("StudentId is required.")
                .MustAsync(async (id, cancellation) =>
                {
                    return await _studentRepository.GetAsync(id) != null;
                }).WithMessage("Student with such id does not exist.");
            
            RuleFor(x => x.ClassroomId)
                .NotEmpty().WithMessage("ClassroomId is required.")
                .MustAsync(async (id, cancellation) =>
                {
                    return await _classroomRepository.GetAsync(id) != null;
                }).WithMessage("Classroom with such id does not exist.");   

            RuleFor(x => new {sid = x.StudentId,cid =  x.ClassroomId})
                .MustAsync(async (dto, cancellation) =>
                {
                    var classroom = await _classroomRepository.GetAsync(dto.cid);
                    if (classroom.Members == null || classroom.Members.Count == 0   )
                    {
                        return false;
                    }
    
                    var members = classroom.Members.Select(x => x.Id).ToList();
                    return members.Contains(dto.sid);
                }).WithMessage("Student is not in the classroom.");
        }
    }
}