using CSharpDiscordWebhook.NET.Discord;
using Microsoft.EntityFrameworkCore;
using Quartz;
using Sentry;
using System.Drawing;
using ZDC.Jobs.Models;
using ZDC.Jobs.Services;
using ZDC.Jobs.Services.Interfaces;
using ZDC.Shared.Models;

namespace ZDC.Jobs.Jobs;

public class DatafeedJob : IJob
{
#nullable disable

    private ILogger<JobsService> _logger;
    private DatabaseContext _context;
    private IDatafeedService _datafeedService;
    private DiscordWebhook _webhook;

#nullable restore

    public async Task Execute(IJobExecutionContext context)
    {
        var scheduler = context.Scheduler.Context;
        _logger = (ILogger<JobsService>)scheduler.Get("Logger");
        _context = (DatabaseContext)scheduler.Get("DatabaseContext");
        _datafeedService = (IDatafeedService)scheduler.Get("DatafeedService");
        var config = (IConfiguration)scheduler.Get("Configuration");
        _webhook = new DiscordWebhook
        {
            Url = config.GetValue<string>("DiscordWebhook")
        };

        _logger.LogInformation("Running datafeed job...");

        var start = DateTimeOffset.UtcNow;
        try
        {
            await UpdateControllerLogs();
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine(ex.Message);
        }
        var end = DateTimeOffset.UtcNow;

        _logger.LogInformation("Datafeed job finished -- Took {time} seconds", Math.Round((end - start).TotalSeconds, 2));
    }

    public async Task UpdateControllerLogs()
    {
        try
        {
            var datafeed = await _datafeedService.GetDatafeed();
            if (datafeed == null || datafeed.Controllers == null || datafeed.Pilots == null)
                return;

            if (datafeed.Controllers.Count == 0)
            {
                await UpdateTimes();
                return;
            }

            var controllers = await _datafeedService.GetZdcControllers(datafeed);

            await RemoveOldLogs(controllers);
            await UpdateOrAddControllerLogs(controllers);
            await UpdateOnlineControllers(controllers);
        }
        catch (Exception ex)
        {
            SentrySdk.CaptureException(ex);
        }
    }

    public async Task UpdateTimes()
    {
        foreach (var entry in _context.ControllerLogs.ToList())
        {
            entry.End = DateTimeOffset.UtcNow;
        }
        await _context.SaveChangesAsync();
    }

    public async Task RemoveOldLogs(IList<Controller> controllers)
    {
        foreach (var entry in _context.ControllerLogs.Where(x => x.Duration == TimeSpan.Zero).ToList())
        {
            var oneMinuteAgo = DateTimeOffset.UtcNow.AddMinutes(-1);
            if (entry.End > oneMinuteAgo)
                continue;
            var log = controllers
                .Where(x => x.Cid == entry.UserId)
                .Where(x => x.Login == entry.Start)
                .Where(x => x.Frequency == entry.Frequency)
                .FirstOrDefault();
            if (log == null)
            {
                entry.Duration = entry.End - entry.Start;

                await SendLogoffWebhook(entry);

                var monthHours = await _context.Hours
                    .Where(x => x.UserId == entry.UserId)
                    .Where(x => x.Month == DateTimeOffset.UtcNow.Month)
                    .Where(x => x.Year == DateTimeOffset.UtcNow.Year)
                    .FirstOrDefaultAsync();

                switch (entry.Type)
                {
                    case ControllerLogType.Local:
                        if (monthHours == null)
                            await _context.Hours.AddAsync(new Hours
                            {
                                UserId = entry.UserId,
                                Month = DateTimeOffset.UtcNow.Month,
                                Year = DateTimeOffset.UtcNow.Year,
                                LocalHours = entry.Duration
                            });
                        else
                            monthHours.LocalHours += entry.Duration;
                        break;
                    case ControllerLogType.Tracon:
                        if (monthHours == null)
                            await _context.Hours.AddAsync(new Hours
                            {
                                UserId = entry.UserId,
                                Month = DateTimeOffset.UtcNow.Month,
                                Year = DateTimeOffset.UtcNow.Year,
                                TraconHours = entry.Duration
                            });
                        else
                            monthHours.TraconHours += entry.Duration;
                        break;
                    case ControllerLogType.Center:
                        if (monthHours == null)
                            await _context.Hours.AddAsync(new Hours
                            {
                                UserId = entry.UserId,
                                Month = DateTimeOffset.UtcNow.Month,
                                Year = DateTimeOffset.UtcNow.Year,
                                CenterHours = entry.Duration
                            });
                        else
                            monthHours.CenterHours += entry.Duration;
                        break;
                }
            }
        }
        await _context.SaveChangesAsync();
    }

    public async Task UpdateOrAddControllerLogs(IList<Controller> controllers)
    {
        foreach (var entry in controllers)
        {
            var log = await _context.ControllerLogs
                .Where(x => x.UserId == entry.Cid)
                .Where(x => x.Start == entry.Login)
                .Where(x => x.Frequency == entry.Frequency)
                .FirstOrDefaultAsync();
            if (log != null)
            {
                log.End = DateTimeOffset.UtcNow;
            }
            else
            {
                var user = await _context.Users.FindAsync(entry.Cid);
                if (user == null)
                {
                    _logger.LogInformation("Non ZDC User logged in -- CID: {Cid}", entry.Cid);
                    continue;
                }

                var newLog = new ControllerLog
                {
                    UserId = entry.Cid,
                    Callsign = entry.Callsign,
                    Frequency = entry.Frequency,
                    Start = entry.Login,
                    End = DateTimeOffset.UtcNow,
                    Duration = TimeSpan.Zero
                };

                if (entry.Callsign.Contains("DEL") || entry.Callsign.Contains("GND") || entry.Callsign.Contains("TWR"))
                    newLog.Type = ControllerLogType.Local;
                if (entry.Callsign.Contains("APP") || entry.Callsign.Contains("DEP"))
                    newLog.Type = ControllerLogType.Tracon;
                if (entry.Callsign.Contains("CTR"))
                    newLog.Type = ControllerLogType.Center;

                await _context.ControllerLogs.AddAsync(newLog);

                await SendLoginWebhook(newLog);
            }
        }
        await _context.SaveChangesAsync();
    }

    public async Task UpdateOnlineControllers(IList<Controller> controllers)
    {
        foreach (var entry in _context.OnlineControllers)
        {
            _context.OnlineControllers.Remove(entry);
        }
        await _context.SaveChangesAsync();

        foreach (var entry in controllers)
        {
            var user = await _context.Users.FindAsync(entry.Cid);
            if (user == null)
                continue;
            var duration = DateTimeOffset.UtcNow - entry.Login;
            await _context.OnlineControllers.AddAsync(new OnlineController
            {
                User = user.FullName,
                Callsign = entry.Callsign,
                Frequency = entry.Frequency,
                Duration = $"{duration.Days * 24 + duration.Hours}h {duration.Minutes}m"
            });
        }
        await _context.SaveChangesAsync();
    }

    public async Task SendLoginWebhook(ControllerLog log)
    {
        var user = await _context.Users.FindAsync(log.UserId);
        if (user != null)
        {
            var embed = new DiscordEmbed
            {
                Title = $"{log.Callsign.ToUpper()} - Online"
            };
            embed.Fields = new List<EmbedField>
            {
                new EmbedField
                {
                    Name = "Name",
                    Value = user.FullName,
                    InLine = true,
                },
                new EmbedField
                {
                    Name = "Rating",
                    Value = user.RatingLong,
                    InLine = true,
                },
                new EmbedField
                {
                    Name = "CID",
                    Value = $"{user.Id}",
                    InLine = true,
                }
            };
            embed.Color = Color.Green;
            embed.Footer = new EmbedFooter
            {
                Text = $"vZDC Staffup Bot - {DateTimeOffset.UtcNow:MM/dd/yyyy HH:mm}"
            };

            var message = new DiscordMessage();
            message.Embeds = new List<DiscordEmbed>
            {
                embed
            };
            _webhook.Send(message);
        }
    }

    public async Task SendLogoffWebhook(ControllerLog log)
    {
        var user = await _context.Users.FindAsync(log.UserId);
        if (user != null)
        {
            var embed = new DiscordEmbed
            {
                Title = $"{log.Callsign.ToUpper()} - Offline"
            };
            embed.Fields = new List<EmbedField>
            {
                new EmbedField
                {
                    Name = "Name",
                    Value = user.FullName,
                    InLine = true,
                },
                new EmbedField
                {
                    Name = "Rating",
                    Value = user.RatingLong,
                    InLine = true,
                },
                new EmbedField
                {
                    Name = "CID",
                    Value = $"{user.Id}",
                    InLine = true,
                }
            };
            embed.Color = Color.Red;
            embed.Footer = new EmbedFooter
            {
                Text = $"vZDC Staffup Bot - {DateTimeOffset.UtcNow:MM/dd/yyyy HH:mm}"
            };

            var message = new DiscordMessage();
            message.Embeds = new List<DiscordEmbed>
            {
                embed
            };
            _webhook.Send(message);
        }
    }
}