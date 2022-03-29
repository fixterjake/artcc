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
}