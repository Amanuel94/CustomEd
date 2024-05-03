namespace CustomEd.Classroom.Service.DTOs
{
    public class RemoveStudentDto
    {
        public Guid StudentId { get; set; }
        public Guid ClassroomId { get; set; }
    }
}