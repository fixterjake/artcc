using System.Net;
using Microsoft.EntityFrameworkCore;
using ZDC.Server.Data;
using ZDC.Server.Repositories.Interfaces;
using ZDC.Shared.Dtos;
using ZDC.Shared.Models;

namespace ZDC.Server.Repositories;

public class DebugLogRepository : IDebugLogRepository
{
    private readonly DatabaseContext _context;

    public DebugLogRepository(DatabaseContext context)
    {
        _context = context;
    }

    public async Task<Response<IList<DebugLog>>> GetDebugLogs()
    {
        var logs = await _context.DebugLogs.ToListAsync();
        return new Response<IList<DebugLog>>
        {
            StatusCode = HttpStatusCode.OK,
            Message = $"Got {logs.Count} debug logs",
            Data = logs
        };
    }
}