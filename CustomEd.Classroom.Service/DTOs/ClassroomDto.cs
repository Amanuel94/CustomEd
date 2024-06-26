using System.Collections.Generic;
using CustomEd.Classroom.Service.Model;

namespace CustomEd.Classroom.Service.DTOs
{
    public class ClassroomDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string CourseNo { get; set; } = null!;
        public string Description { get; set; } = null!;
        public Teacher Creator { get; set; } = null!;
        public List<Student> Members { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
