using Microsoft.EntityFrameworkCore;
using System.Net;
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
    public async Task<ResponsePaging<IList<EmailLog>>> GetEmailLogs(int skip, int take)
    {
        var result = await _context.EmailLogs.Skip(skip).Take(take).ToListAsync();
        var totalCount = await _context.EmailLogs.CountAsync();
        return new ResponsePaging<IList<EmailLog>>
        {
            StatusCode = HttpStatusCode.OK,
            TotalCount = totalCount,
            ResultCount = result.Count,
            Message = $"Got {result.Count} email logs",
            Data = result
        };
    }
}
