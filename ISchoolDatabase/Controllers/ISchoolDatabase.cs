using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace CustomEd.ISchoolDatabase.Controllers
{
    [ApiController]
    [Route("schooldb")]
    public class SchoolDatabaseController : ControllerBase
    {
        private readonly List<string> StudentEmails = new List<string>()
        {
            "alex.johnson@example.com",
            "emily.brown@example.net",
            "michael.smith@example.org",
            "sophia.wilson@example.com",
            "james.martin@example.net",
            "mia.miller@example.org",
            "william.taylor@example.com",
            "isabella.anderson@example.net",
            "benjamin.thomas@example.org",
            "amelia.jackson@example.com",
            "lucas.white@example.net",
            "olivia.harris@example.org",
            "henry.thompson@example.com",
            "ava.roberts@example.net",
            "ethan.jones@example.org",
            "charlotte.lewis@example.com",
            "liam.clark@example.net",
            "harper.walker@example.org",
            "noah.hall@example.com",
            "evelyn.allen@example.net",
            "mason.young@example.org",
            "grace.king@example.com",
            "logan.wright@example.net",
            "scarlett.scott@example.org",
            "jacob.green@example.com",
            "lily.adams@example.net",
            "sebastian.baker@example.org",
            "zoe.mitchell@example.com",
            "jack.carter@example.net",
            "layla.phillips@example.org"
        };

        private readonly List<string> TeacherEmails = new List<string>()
        {
            "john.doe@example.com",
            "jane.doe@example.net",
            "mary.jane@example.org",
            "paul.smith@example.com",
            "linda.jones@example.net"
        };


        [HttpGet("students/emails")]
        public ActionResult<List<string>> GetStudentEmails()
        {
            return Ok(StudentEmails);
        }

       

        [HttpDelete("students/emails")]
        public ActionResult RemoveStudentEmail([FromBody] string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return BadRequest("Email cannot be null or empty.");
            }

            if (StudentEmails.Remove(email))
            {
                return Ok("Email removed successfully.");
            }

            return NotFound("Email not found.");
        }


        [HttpGet("teachers/emails")]
        public ActionResult<List<string>> GetTeacherEmails()
        {
            return Ok(TeacherEmails);
        }

    

        [HttpDelete("teachers/emails")]
        public ActionResult RemoveTeacherEmail([FromBody] string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return BadRequest("Email cannot be null or empty.");
            }

            if (TeacherEmails.Remove(email))
            {
                return Ok("Email removed successfully.");
            }

            return NotFound("Email not found.");
        }

        [HttpGet("emails/{email}")]
        public ActionResult<bool> CheckEmailExists(string email)
        {
            bool emailExists = StudentEmails.Contains(email) || TeacherEmails.Contains(email);
            return Ok(emailExists);
        }
    }

}
