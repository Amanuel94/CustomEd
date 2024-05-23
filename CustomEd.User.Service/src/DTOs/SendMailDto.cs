using System;
using System.Collections.Generic;

namespace CustomEd.User.Service.DTOs
{
    public class SendMailDto
    {
        public List<Guid> TeacherIds { get; set; } = null!;
        public string Subject { get; set; } = null!;
        public string Message { get; set; } = null!;
    }
}
