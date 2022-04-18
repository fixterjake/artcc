using Microsoft.EntityFrameworkCore;
using ZDC.Server.Data;
using ZDC.Server.Repositories.Interfaces;
using ZDC.Shared.Dtos;
using ZDC.Shared.Models;

namespace ZDC.Server.Repositories;

public class EmailLogRepository : IEmailLogRepository
{

    private readonly DatabaseContext _context;

    public EmailLogRepository(DatabaseContext context)
    {
        _context = context;
    }

    /// <inheritdoc />
    public async Task<Response<IList<EmailLog>>> GetEmailLogs(int skip, int take)
    {
        var result = await _context.EmailLogs.Skip(skip).Take(take).ToListAsync();
        return new Response<IList<EmailLog>>
        {
            StatusCode = System.Net.HttpStatusCode.OK,
            Message = $"Got {result.Count} email logs",
            Data = result
        };
    }
}
