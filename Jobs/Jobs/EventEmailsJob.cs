using Quartz;
using ZDC.Jobs.Services.Interfaces;
using ZDC.Jobs.Services;
using Microsoft.EntityFrameworkCore;

namespace ZDC.Jobs.Jobs;

public class EventEmailsJob : IJob
{

#nullable disable

    private ILogger<JobsService> _logger;
    private DatabaseContext _context;
    private ILoggingService _loggingService;
    private IEmailService _emailService;

#nullable restore

    public async Task Execute(IJobExecutionContext context)
    {
        var scheduler = context.Scheduler.Context;
        _logger = (ILogger<JobsService>)scheduler.Get("Logger");
        _context = (DatabaseContext)scheduler.Get("DatabaseContext");
        _loggingService = (ILoggingService)scheduler.Get("LoggingService");
        _emailService = (IEmailService)scheduler.Get("EmailService");


        _logger.LogInformation("Running event emails job...");

        var start = DateTimeOffset.UtcNow;
        await SendEventEmails();
        var end = DateTimeOffset.UtcNow;

        _logger.LogInformation("Event emails job finished -- Took {time} seconds", Math.Round((end - start).TotalSeconds, 2));
    }

    public async Task SendEventEmails()
    {
        try
        {
            var events = _context.Events.Where(x => x.Start <= DateTimeOffset.UtcNow - TimeSpan.FromDays(24)).ToList();
            foreach (var entry in events)
            {
                var registrations = await _context.EventsRegistrations
                    .Include(x => x.User)
                    .Include(x => x.Position)
                    .Where(x => x.EventId == entry.Id)
                    .ToListAsync();
                foreach (var registration in registrations)
                {
                    if (registration.User == null || registration.Position == null)
                    {
                        _logger.LogError("Registration {id} had a null user or position", registration.Id);
                        continue;
                    }
                    await _emailService.SendEventReminder(registration.User.Email, entry.Id, entry.Name,
                        registration.Position.Name, registration.Start.ToString("MM/dd/yyyy HH:mm"), registration.End.ToString("MM/dd/yyyy HH:mm"));
                }
            }
        }
        catch (Exception ex)
        {
            await _loggingService.AddDebugLog(ex.Message, ex.StackTrace ?? "N/A");
        }
    }
}
