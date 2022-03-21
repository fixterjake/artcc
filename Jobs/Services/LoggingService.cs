using ZDC.Jobs.Services.Interfaces;
using ZDC.Shared.Models;

namespace ZDC.Jobs.Services;

public class LoggingService : ILoggingService
{
    private readonly DatabaseContext _context;

    public LoggingService(DatabaseContext context)
    {
        _context = context;
    }

    public async Task AddWebsiteLog(string action, string oldData, string newData)
    {
        await _context.WebsiteLogs.AddAsync(new WebsiteLog
        {
            Ip = "N/A",
            Cid = "N/A",
            Name = "SYSTEM",
            Action = action,
            OldData = oldData,
            NewData = newData
        });
        await _context.SaveChangesAsync();
    }

    public async Task AddDebugLog(string exception, string stackTrace)
    {
        var result = await _context.DebugLogs.AddAsync(new DebugLog
        {
            Ip = "N/A",
            Cid = "N/A",
            Name = "SYSTEM",
            Route = "N/A",
            Exception = exception,
            StackTrace = stackTrace
        });
        await _context.SaveChangesAsync();
    }
}