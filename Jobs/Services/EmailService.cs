using System.Net.Mail;
using System.Net;
using ZDC.Jobs.Services.Interfaces;
using ZDC.Shared.Models;
using File = System.IO.File;

namespace ZDC.Jobs.Services;

public class EmailService : IEmailService
{
    private readonly DatabaseContext _context;
    private readonly IConfiguration _config;

    public EmailService(DatabaseContext context, IConfiguration config)
    {
        _context = context;
        _config = config;
    }

    public async Task<string> LoadEmailTemplate(string name)
    {
        return await File.ReadAllTextAsync($"./Emails/build_production/{name}.html");
    }

    public async Task<bool> SendEmail(string type, string to, string fromName, string subject, string body)
    {
        var emailConfig = _config.GetSection("EmailOptions");

        var mail = new MailMessage();
        mail.IsBodyHtml = true;
        mail.From = new MailAddress(emailConfig.GetValue<string>("From"), fromName);
        mail.To.Add(to);
        mail.Subject = subject;
        mail.Body = body;
        mail.Priority = MailPriority.High;

        using var client = new SmtpClient(emailConfig.GetValue<string>("Host"), emailConfig.GetValue<int>("Port"));
        client.Credentials = new NetworkCredential(emailConfig.GetValue<string>("Username"),
            emailConfig.GetValue<string>("Password"));
        client.EnableSsl = true;

        try
        {
            await client.SendMailAsync(mail);
            await _context.EmailLogs.AddAsync(new EmailLog
            {
                Email = type,
                To = to
            });
            await _context.SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error sending email: {ex.Message}\n{ex.StackTrace}");
            return false;
        }
    }

    public async Task<bool> SendEventReminder(string to, int eventId, string name, string position, string start, string end)
    {
        var template = await LoadEmailTemplate("EventReminder");
        template = template.Replace("{Name}", name);
        template = template.Replace("{Position}", position);
        template = template.Replace("{Start}", start);
        template = template.Replace("{End}", end);
        template = template.Replace("{Link}", $"https://vzdc.xyz/event/{eventId}");
        return await SendEmail("EventReminder", to, "vZDC Events", "vZDC Event Reminder", template);
    }
}
