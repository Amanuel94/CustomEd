using CustomEd.Shared.Model;

namespace ISchoolDatabase.Models
{
    public class Student
    {
        public string StudentEmail { get; set; }
        public int Department { get; set; }
        public int Year { get; set; }
        public string Section { get; set; }
    }

    public class Teacher
    {
        public string TeacherEmail { get; set; }
        public int Department { get; set; }
        public List<string> CourseNumbers { get; set; }
    }

    public class DBSyncDto
    {
        public List<Student> StudentProfiles { get; set; }
        public List<Teacher> TeacherProfiles { get; set; }
    }

}