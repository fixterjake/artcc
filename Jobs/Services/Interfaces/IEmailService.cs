namespace ZDC.Jobs.Services.Interfaces;

public interface IEmailService
{
    Task<string> LoadEmailTemplate(string name);
    Task<bool> SendEmail(string type, string to, string fromName, string subject, string body);
    Task<bool> SendEventReminder(string to, int eventId, string name, string position, string start, string end);
}
