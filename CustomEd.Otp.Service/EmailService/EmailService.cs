using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Options;

namespace CustomEd.Otp.Service.Email.Service;

public class EmailService : IEmailService
{
    private readonly SmtpClient _smtpClient;
    private readonly IOptions<EmailSettings> _emailSettings;

    public EmailService(IOptions<EmailSettings> emailSettings)
    {
        _emailSettings = emailSettings;
        _smtpClient = new SmtpClient
        {
            Host = _emailSettings.Value.MailServer, 
            Port = _emailSettings.Value.MailPort, 
            EnableSsl = true,
            Credentials = new NetworkCredential(_emailSettings.Value.Username, _emailSettings.Value.Password) 
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
