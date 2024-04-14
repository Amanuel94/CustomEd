using System.Net;
using System.Net.Mail;

namespace CustomEd.Otp.Service.Email.Service;
public interface IEmailService
{
    Task SendEmailAsync(string email, string subject, string message);
}


