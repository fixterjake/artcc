using System.Net;
using System.Net.Mail;
using ZDC.Server.Data;
using ZDC.Server.Services.Interfaces;
using ZDC.Shared.Models;
using File = System.IO.File;

namespace ZDC.Server.Services;

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

    public async Task<bool> SendEventPositionAssigned(string to, int eventId, string name, string position, DateTimeOffset start, DateTimeOffset end)
    {
        var template = await LoadEmailTemplate("EventPositionAssigned");
        template = template.Replace("{Name}", name);
        template = template.Replace("{Position}", position);
        template = template.Replace("{Start}", start.ToString("MM/dd/yyyy HH:mm"));
        template = template.Replace("{End}", end.ToString("MM/dd/yyyy HH:mm"));
        template = template.Replace("{Link}", $"https://vzdc.xyz/event/{eventId}");
        return await SendEmail("EventPositionAssigned", to, "vZDC Events", "vZDC Event Position Assigned", template);
    }

    public async Task<bool> SendEventRegistrationSubmitted(string to, int eventId, string name, DateTimeOffset start, DateTimeOffset end)
    {
        var template = await LoadEmailTemplate("EventRegistrationSubmitted");
        template = template.Replace("{Name}", name);
        template = template.Replace("{Start}", start.ToString("MM/dd/yyyy HH:mm"));
        template = template.Replace("{End}", end.ToString("MM/dd/yyyy HH:mm"));
        template = template.Replace("{Link}", $"https://vzdc.xyz/event/{eventId}");
        return await SendEmail("EventRegistrationSubmitted", to, "vZDC Events", "vZDC Event Registration Submitted", template);
    }

    public async Task<bool> SendEventRegistrationRemoved(string to, int eventId, string name)
    {
        var template = await LoadEmailTemplate("EventRegistrationRemoved");
        template = template.Replace("{Name}", name);
        template = template.Replace("{Name}", name);
        template = template.Replace("{Link}", $"https://vzdc.xyz/event/{eventId}");
        return await SendEmail("EventRegistrationRemoved", to, "vZDC Events", "vZDC Event Registration Removed", template);
    }

    public async Task<bool> SendFeedbackApproved(string to, string position, string callsign, string comments, string staffComments)
    {
        var template = await LoadEmailTemplate("FeedbackApproved");
        template = template.Replace("{Position}", position);
        template = template.Replace("{Callsign}", callsign);
        template = template.Replace("{Comments}", comments);
        template = template.Replace("{StaffComments}", staffComments);
        return await SendEmail("FeedbackApproved", to, "vZDC Management", "vZDC New Feedback", template);
    }

    public async Task<bool> SendLoaAccepted(string to, DateTimeOffset start, DateTimeOffset end, string reason)
    {
        var template = await LoadEmailTemplate("LoaAccepted");
        template = template.Replace("{Start}", start.ToString("MM/dd/yyyy HH:mm"));
        template = template.Replace("{End}", end.ToString("MM/dd/yyyy HH:mm"));
        template = template.Replace("{Reason}", reason);
        return await SendEmail("LoaAccepted", to, "vZDC Management", "vZDC LOA Accepted", template);
    }

    public async Task<bool> SendLoaSubmitted(string to, DateTimeOffset start, DateTimeOffset end, string reason)
    {
        var template = await LoadEmailTemplate("LoaSubmitted");
        template = template.Replace("{Start}", start.ToString("MM/dd/yyyy HH:mm"));
        template = template.Replace("{End}", end.ToString("MM/dd/yyyy HH:mm"));
        template = template.Replace("{Reason}", reason);
        return await SendEmail("LoaSubmitted", to, "vZDC Management", "vZDC LOA Submitted", template);
    }

    public async Task<bool> SendLoaDenied(string to, string reason)
    {
        var template = await LoadEmailTemplate("LoaDenied");
        template = template.Replace("{Reason}", reason);
        return await SendEmail("LoaDenied", to, "vZDC Management", "vZDC LOA Denied", template);
    }

    public async Task<bool> SendLoaEnded(string to)
    {
        var template = await LoadEmailTemplate("LoaEnded");
        return await SendEmail("LoaEnded", to, "vZDC Management", "vZDC LOA Ended", template);
    }

    public async Task<bool> SendNewFeedback(string to, int feedbackId, string position, string callsign, string comments)
    {
        var template = await LoadEmailTemplate("NewFeedback");
        template = template.Replace("{Position}", position);
        template = template.Replace("{Callsign}", callsign);
        template = template.Replace("{Comments}", comments);
        template = template.Replace("{Link}", $"https://vzdc.xyz/feedback/{feedbackId}");
        return await SendEmail("NewFeedback", to, "vZDC Management", "vZDC New Feedback", template);
    }

    public async Task<bool> SendNewLoa(string to, int loaId, string name, DateTimeOffset start, DateTimeOffset end, string reason)
    {
        var template = await LoadEmailTemplate("NewLoa");
        template = template.Replace("{Name}", name);
        template = template.Replace("{Start}", start.ToString("MM/dd/yyyy HH:mm"));
        template = template.Replace("{End}", end.ToString("MM/dd/yyyy HH:mm"));
        template = template.Replace("{Reason}", reason);
        template = template.Replace("{Link}", $"https://vzdc.xyz/loa/{loaId}");
        return await SendEmail("NewLoa", to, "vZDC Management", "vZDC New LOA", template);
    }

    public async Task<bool> SendNewOts(string to, int otsId, string name, string cid, string recommender, string rating, string facility, string position)
    {
        var template = await LoadEmailTemplate("NewOts");
        template = template.Replace("{Name}", name);
        template = template.Replace("{Cid}", cid);
        template = template.Replace("{Recommender}", recommender);
        template = template.Replace("{Rating}", rating);
        template = template.Replace("{Facility}", facility);
        template = template.Replace("{Position}", position);
        template = template.Replace("{Link}", $"https://vzdc.xyz/ots/{otsId}");
        return await SendEmail("NewOts", to, "vZDC Management", "vZDC New OTS", template);
    }

    public async Task<bool> SendNewStaffingRequest(string to, int staffingRequestId, string name, string email, string affiliation, DateTimeOffset start, DateTimeOffset end, string description)
    {
        var template = await LoadEmailTemplate("NewStaffingRequest");
        template = template.Replace("{Name}", name);
        template = template.Replace("{Email}", email);
        template = template.Replace("{Affiliation}", affiliation);
        template = template.Replace("{Start}", start.ToString("MM/dd/yyyy HH:mm"));
        template = template.Replace("{End}", end.ToString("MM/dd/yyyy HH:mm"));
        template = template.Replace("{Description}", description);
        template = template.Replace("{Link}", $"https://vzdc.xyz/ots/{staffingRequestId}");
        return await SendEmail("NewStaffingRequest", to, "vZDC Events", "vZDC New Staffing Request", template);
    }

    public async Task<bool> SendNewVisitRequest(string to, int visitRequestId, string name, string cid, string rating, string reason, string requirements)
    {
        var template = await LoadEmailTemplate("NewVisitRequest");
        template = template.Replace("{Name}", name);
        template = template.Replace("{Cid}", cid);
        template = template.Replace("{Rating}", rating);
        template = template.Replace("{Reason}", reason);
        template = template.Replace("{Requirements}", requirements);
        template = template.Replace("{Link}", $"https://vzdc.xyz/visit/{visitRequestId}");
        return await SendEmail("NewVisitRequest", to, "vZDC Management", "vZDC New Visit Request", template);
    }

    public async Task<bool> SendOtsAssigned(string to, int otsId, string name, string cid, string rating, string facility, string position)
    {
        var template = await LoadEmailTemplate("OtsAssigned");
        template = template.Replace("{Name}", name);
        template = template.Replace("{Cid}", cid);
        template = template.Replace("{Rating}", rating);
        template = template.Replace("{Facility}", facility);
        template = template.Replace("{Position}", position);
        template = template.Replace("{Link}", $"https://vzdc.xyz/ots/{otsId}");
        return await SendEmail("OtsAssigned", to, "vZDC Management", "vZDC New OTS Assignment", template);
    }

    public async Task<bool> SendStaffingRequestSubmitted(string to, string name, string email, string affiliation, DateTimeOffset start, DateTimeOffset end, string description)
    {
        var template = await LoadEmailTemplate("StaffingRequestSubmitted");
        template = template.Replace("{Name}", name);
        template = template.Replace("{Email}", email);
        template = template.Replace("{Affiliation}", affiliation);
        template = template.Replace("{Start}", start.ToString("MM/dd/yyyy HH:mm"));
        template = template.Replace("{End}", end.ToString("MM/dd/yyyy HH:mm"));
        template = template.Replace("{Description}", description);
        return await SendEmail("StaffingRequestSubmitted", to, "vZDC Events", "vZDC Staffing Request Submitted", template);
    }

    public async Task<bool> SendVisitRequestAccepted(string to)
    {
        var template = await LoadEmailTemplate("VisitRequestAccepted");
        template = template.Replace("{Website}", _config.GetSection("EmailOptions").GetValue<string>("Website"));
        return await SendEmail("VisitRequestAccepted", to, "vZDC Management", "vZDC Visit Request Accepted", template);
    }

    public async Task<bool> SendVisitRequestDenied(string to, string reason)
    {
        var template = await LoadEmailTemplate("VisitRequestDenied");
        template = template.Replace("{Reason}", reason);
        return await SendEmail("VisitRequestDenied", to, "vZDC Management", "vZDC Visit Request Denied", template);
    }
}