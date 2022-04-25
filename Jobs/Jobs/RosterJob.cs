using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Quartz;
using Sentry;
using ZDC.Jobs.Models;
using ZDC.Jobs.Services;
using ZDC.Jobs.Services.Interfaces;
using ZDC.Shared.Models;
using User = ZDC.Shared.Models.User;

namespace ZDC.Jobs.Jobs;

public class RosterJob : IJob
{
#nullable disable

    private ILogger<JobsService> _logger;
    private DatabaseContext _context;
    private ILoggingService _loggingService;
    private IVatusaService _vatusaService;

#nullable restore

    public async Task Execute(IJobExecutionContext context)
    {
        var scheduler = context.Scheduler.Context;
        _logger = (ILogger<JobsService>)scheduler.Get("Logger");
        _context = (DatabaseContext)scheduler.Get("DatabaseContext");
        _loggingService = (ILoggingService)scheduler.Get("LoggingService");
        _vatusaService = (IVatusaService)scheduler.Get("VatusaService");


        _logger.LogInformation("Running roster job...");

        var start = DateTimeOffset.UtcNow;
        try
        {
            await UpdateRoster();
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine(ex.Message);
        }
        var end = DateTimeOffset.UtcNow;

        _logger.LogInformation("Roster job finished -- Took {time} seconds", Math.Round((end - start).TotalSeconds, 2));
    }

    public async Task UpdateRoster()
    {
        try
        {
            var roster = await _vatusaService.GetRoster();
            if (roster == null || roster.Data == null)
                return;

            await UpdateUsers(roster.Data);
            await AddNewUsers(roster.Data);
            await RemoveUsers(roster.Data);
        }
        catch (Exception ex)
        {
            SentrySdk.CaptureException(ex);
        }
    }

    public async Task<string> GetInitials(string firstName, string lastName)
    {
        var initials = $"{firstName[0]}{lastName[0]}";

        var initialsExist = await _context.Users
            .Where(x => x.Initials.Equals(initials))
            .ToListAsync();

        if (!initialsExist.Any()) return initials;

        foreach (var letter in lastName)
        {
            initials = $"{firstName[0]}{letter.ToString().ToUpper()}";

            var exists = await _context.Users
                .Where(x => x.Initials.Equals(initials))
                .ToListAsync();

            if (!exists.Any()) return initials.ToUpper();
        }

        foreach (var letter in firstName)
        {
            initials = $"{letter.ToString().ToUpper()}{lastName[0]}";

            var exists = await _context.Users
                .Where(x => x.Initials.Equals(initials))
                .ToListAsync();

            if (!exists.Any()) return initials.ToUpper();
        }

        return string.Empty;
    }

    public async Task UpdateUsers(IList<RosterEntry> users)
    {
        var changes = new List<string>();
        foreach (var entry in users)
        {
            var user = await _context.Users.FindAsync(entry.Cid);
            if (user == null)
                continue;

            var oldData = JsonConvert.SerializeObject(user);

            if (user.FirstName != entry.FirstName)
            {
                user.FirstName = entry.FirstName;
                changes.Add("Updated First Name");
            }

            if (user.LastName != entry.LastName)
            {
                user.LastName = entry.LastName;
                changes.Add("Updated Last Name");
            }

            if (user.Email != entry.Email)
            {
                user.Email = entry.Email;
                changes.Add("Updated Email");
            }

            if (user.Rating != entry.Rating)
            {
                user.Rating = entry.Rating;
                changes.Add("Updated Rating");
            }

            if (!user.Visitor && (entry.Membership != "home"))
            {
                user.Visitor = true;
                user.VisitorFrom = entry.Facility;
                changes.Add("Updated Visitor Status");
            }

            if (user.Visitor && (entry.Membership == "home"))
            {
                user.Visitor = false;
                user.VisitorFrom = String.Empty;
                changes.Add("Updated Visitor Status");
            }
            user.Updated = DateTimeOffset.UtcNow;

            await _context.SaveChangesAsync();
            if (changes.Count > 0)
            {
                var newData = JsonConvert.SerializeObject(user);
                var actions = string.Join(",", changes);
                await _loggingService.AddWebsiteLog(actions, oldData, newData);
            }
        }
    }

    public async Task AddNewUsers(IList<RosterEntry> users)
    {
        foreach (var entry in users)
        {
            var user = await _context.Users.FindAsync(entry.Cid);
            if (user != null)
                continue;
            var result = await _context.Users.AddAsync(new User
            {
                Id = entry.Cid,
                FirstName = entry.FirstName,
                LastName = entry.LastName,
                Initials = await GetInitials(entry.FirstName, entry.LastName),
                Email = entry.Email ?? string.Empty,
                Rating = entry.Rating,
                Joined = entry.Joined,
                Visitor = entry.Membership == "visit",
                VisitorFrom = entry.Membership == "visit" ? entry.Facility : string.Empty,
            });
            await _context.SaveChangesAsync();
            var newData = JsonConvert.SerializeObject(result.Entity);
            await _loggingService.AddWebsiteLog("Added New User", string.Empty, newData);
        }
    }

    public async Task RemoveUsers(IList<RosterEntry> users)
    {
        foreach (var entry in _context.Users.ToList())
        {
            if (users.Any(x => x.Cid == entry.Id))
                continue;
            var oldData = JsonConvert.SerializeObject(entry);
            entry.Status = UserStatus.Removed;
            entry.Updated = DateTimeOffset.UtcNow;
            await _context.SaveChangesAsync();
            await _loggingService.AddWebsiteLog("Remoevd User", oldData, string.Empty);
        }
    }
}

