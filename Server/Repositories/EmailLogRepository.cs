using Microsoft.EntityFrameworkCore;
using ZDC.Server.Data;
using ZDC.Server.Repositories.Interfaces;
using ZDC.Shared.Models;

namespace ZDC.Server.Repositories;

public class EmailLogRepository : IEmailLogRepository
{

    private readonly DatabaseContext _context;

    public EmailLogRepository(DatabaseContext context)
    {
        _context = context;
    }

    public async Task<IList<EmailLog>> GetEmailLogs(int skip, int take)
    {
        return await _context.EmailLogs.Skip(skip).Take(take).ToListAsync();
    }
}
