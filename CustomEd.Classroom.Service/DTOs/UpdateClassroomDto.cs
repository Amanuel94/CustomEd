using System.Collections.Generic;

namespace CustomEd.Classroom.Service.DTOs
{
    public class UpdateClassroomDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string CourseNo { get; set; } = null!;
        public string Description { get; set; } = null!;
        public Guid CreatorId { get; set; }
    }
}