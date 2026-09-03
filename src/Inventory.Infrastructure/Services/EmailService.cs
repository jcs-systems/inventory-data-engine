using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Options;
using Inventory.Application.Interfaces;
using Inventory.Application.Models;

namespace Inventory.Infrastructure.Services;

public class EmailService : IEmailService
{
    private readonly SmtpSettings _settings;

    public EmailService(IOptions<SmtpSettings> settings)
    {
        _settings = settings.Value;
    }

    public async Task SendEmailAsync(string toEmail, string subject, string body)
    {
        using var client = new SmtpClient(_settings.Server, _settings.Port)
        {
            UseDefaultCredentials = false, // <-- AGREGAR ESTA LÍNEA OBLIGATORIAMENTE
            Credentials = new NetworkCredential(_settings.Username, _settings.Password),
            EnableSsl = true
        };

        var mailMessage = new MailMessage
        {
            From = new MailAddress(_settings.SenderEmail, _settings.SenderName),
            Subject = subject,
            Body = body,
            IsBodyHtml = true
        };

        mailMessage.To.Add(toEmail);

        await client.SendMailAsync(mailMessage);
    }
}