using System.Net;
using System.Net.Mail;

namespace CustomEd.Otp.Service.Email.Service;

public class EmailService : IEmailService
{
    private readonly SmtpClient _smtpClient;

    public EmailService()
    {
        _smtpClient = new SmtpClient
        {
            Host = "smtp.example.com", 
            Port = 587, 
            EnableSsl = true,
            Credentials = new NetworkCredential("username@example.com", "password") 
        };
    }

    public async Task SendEmailAsync(string email, string subject, string message)
    {
        var mailMessage = new MailMessage
        {
            From = new MailAddress("username@example.com"), // Set your email here
            Subject = subject,
            Body = message,
            IsBodyHtml = true,
        };

        mailMessage.To.Add(email);

        await _smtpClient.SendMailAsync(mailMessage);
    }
}
