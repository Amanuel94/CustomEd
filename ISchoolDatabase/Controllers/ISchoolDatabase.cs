using CustomEd.ISchoolDatabase.Dto;
using CustomEd.Shared.Model;
using ISchoolDatabase.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.IO.Pipelines;
using System.Linq;

namespace CustomEd.ISchoolDatabase.Controllers
{


    [ApiController]
    [Route("schooldb")]
    public class SchoolDatabaseController : ControllerBase
    {
        private readonly List<Student> StudentList = new List<Student>()
        {
//             aadmasterefe00@gmail.com 
// admasterefe00@gmail.com 
// frehiwotlema07@gmail.com 
// admas.terefe@aau.edu.et 
// admas.terefe@a2sv.org
// tarikterefe9@gmail.com

            new Student { StudentEmail = "alex.johnson@example.com", Department = 0, Year = 3, Section = "A" },
            new Student { StudentEmail = "emily.brown@example.net", Department = 1, Year = 2, Section = "B" },
            new Student { StudentEmail = "michael.smith@example.org", Department = 1, Year = 4, Section = "C" },
            new Student { StudentEmail = "sophia.wilson@example.com", Department = 0, Year = 1, Section = "A" },
            new Student {StudentEmail = "aadmasterefe00@gmail.com", Department = 0, Year = 1, Section = "A" },
            new Student {StudentEmail = "admasterefe00@gmail.com", Department = 0, Year = 1, Section = "A" },
            new Student {StudentEmail = "frehiwotlema07@gmail.com ", Department = 0, Year = 1, Section = "A" },
            new Student {StudentEmail = "dmas.terefe@aau.edu.et", Department = 0, Year = 1, Section = "A" },
            new Student {StudentEmail = "admas.terefe@a2sv.org", Department = 0, Year = 1, Section = "A" }
        };

        private readonly List<Teacher> TeacherList = new List<Teacher>()
        {
            new Teacher { TeacherEmail = "john.doe@example.com", Department = 0, CourseNumbers = new List<string> { "CS101", "CS102" } },
            new Teacher { TeacherEmail = "jane.doe@example.net", Department = 1, CourseNumbers = new List<string> { "EE101", "EE102" } },
            new Teacher { TeacherEmail = "mary.jane@example.org", Department = 1, CourseNumbers = new List<string> { "ME101", "ME102" } },
            new Teacher { TeacherEmail = "paul.smith@example.com", Department = 0, CourseNumbers = new List<string> { "CE101", "CE102" } },
            new Teacher { TeacherEmail = "tarikterefe9@gmail.com", Department = 0, CourseNumbers = new List<string> { "CT101", "CT102" } }
        };

        [HttpGet("students/emails")]
        public ActionResult<List<string>> GetStudentEmails()
        {
            var emails = StudentList.Select(s => s.StudentEmail).ToList();
            Console.WriteLine(emails);
            return Ok(emails);
        }

        [HttpGet("students/profile/{email}")]
        public ActionResult<List<string>> GetStudentProfile(string email)
        {
            Console.WriteLine(email);
            var student = StudentList.FirstOrDefault(s => s.StudentEmail == email);
            if (student != null)
            {
                var profile = new List<string>
                {
                    student.StudentEmail,
                    student.Department.ToString(),
                    student.Year.ToString(),
                    student.Section
                };
                
                return Ok(profile);
            }


            return NotFound("Student not found.");
        }


        [HttpGet("teachers/profile")]
        public ActionResult<List<string>> GetTeacherProfile(string email)
        {
            var teacher = TeacherList.FirstOrDefault(t => t.TeacherEmail == email);
            if (teacher != null)
            {
                var profile = new List<string>
                {
                    teacher.TeacherEmail,
                    teacher.Department.ToString(),
                    string.Join(", ", teacher.CourseNumbers)
                };
                return Ok(profile);
            }

            return NotFound("Teacher not found.");
        }

        [HttpGet("fetch-database/students")]
        public ActionResult<List<Student>> FetchStudentDatabase()
        {
            var studentProfiles = new List<Student>();
            foreach (var student in StudentList)
            {
                var profile = new Student
                {
                    StudentEmail = student.StudentEmail,
                    Department = student.Department,
                    Year = student.Year,
                    Section = student.Section
                };
                studentProfiles.Add(profile);
            }

           

            return studentProfiles;

        }

        [HttpGet("fetch-database/teachers")]
        public ActionResult<List<Teacher>> FetchSTeacherDatabase()
        {
             var teacherProfiles = new List<Teacher>();
            
            foreach (var teacher in TeacherList)
            {
                var profile = new Teacher
                {
                    TeacherEmail = teacher.TeacherEmail,
                    Department = teacher.Department,
                    CourseNumbers = teacher.CourseNumbers
                };
                teacherProfiles.Add(profile);
            }

            return teacherProfiles;

        }

        [HttpDelete("students/emails")]
        public ActionResult RemoveStudentEmail([FromBody] string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return BadRequest("Email cannot be null or empty.");
            }

            var student = StudentList.FirstOrDefault(s => s.StudentEmail == email);
            if (student != null)
            {
                StudentList.Remove(student);
                return Ok("Email removed successfully.");
            }

            return NotFound("Email not found.");
        }

        [HttpGet("teachers/emails")]
        public ActionResult<List<string>> GetTeacherEmails()
        {
            var emails = TeacherList.Select(t => t.TeacherEmail).ToList();
            return Ok(emails);
        }

        [HttpDelete("teachers/emails")]
        public ActionResult RemoveTeacherEmail([FromBody] string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return BadRequest("Email cannot be null or empty.");
            }

            var teacher = TeacherList.FirstOrDefault(t => t.TeacherEmail == email);
            if (teacher != null)
            {
                TeacherList.Remove(teacher);
                return Ok("Email removed successfully.");
            }

            return NotFound("Email not found.");
        }

        [HttpGet("emails/{email}")]
        public ActionResult<SchoolResonseDto> CheckEmailExists(string email)
        {
            Console.WriteLine(email);
            var student = StudentList.FirstOrDefault(s => s.StudentEmail == email);
            var teacher = TeacherList.FirstOrDefault(t => t.TeacherEmail == email);
            if (student != null)
            {
                var response = new SchoolResonse
                {
                    UserExists = true,
                    Role = Role.Student
                };
                return Ok(response);
            }
            else if (teacher != null)
            {
                var response = new SchoolResonse
                {
                    UserExists = true,
                    Role = Role.Teacher
                };
                return Ok(response);
            }
            else
            {
                var response = new SchoolResonse
                {
                    UserExists = false
                };
                return Ok(response);
            }
        }
    }

    internal class SchoolResonse
    {
        public bool UserExists { get; set; }
        public Role Role { get; set; }
    }
}
