using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Mail;
using TesteVsoft.Application.Interfaces.Services;
using TesteVsoft.Infrastructure.Common.Attributes;

[Scoped]
public class EmailService(IConfiguration configuration) : IEmailService
{
    public async Task SendAsync(string to, string subject, string htmlBody)
    {
        var message = new MailMessage
        {
            From = new MailAddress(configuration["MailSettings:From"]!),
            Subject = subject,
            Body = htmlBody,
            IsBodyHtml = true,
            To = { new MailAddress(to) }
        };

        message.To.Add(to);

        var client = new SmtpClient(configuration["MailSettings:Host"], int.Parse(configuration["MailSettings:Port"]!))
        {
            Credentials = new NetworkCredential(configuration["MailSettings:User"], configuration["MailSettings:Password"]),
            EnableSsl = true
        };

        await client.SendMailAsync(message);
    }
}
