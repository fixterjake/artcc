using System.Net;
using Microsoft.EntityFrameworkCore;
using ZDC.Server.Data;
using ZDC.Server.Repositories.Interfaces;
using ZDC.Shared;
using ZDC.Shared.Dtos;
using ZDC.Shared.Models;

namespace ZDC.Server.Repositories;

public class ControllerLogRepository : IControllerLogRepository
{
    private readonly DatabaseContext _context;

    public ControllerLogRepository(DatabaseContext context)
    {
        _context = context;
    }

    public async Task<Response<IList<ControllerLog>>> GetUserControllerLogs(int id)
    {
        if (!_context.Users.Any(x => x.Id == id))
            throw new UserNotFoundException($"User '{id}' not found");
        var logs = await _context.ControllerLogs.Where(x => x.UserId == id).ToListAsync();
        return new Response<IList<ControllerLog>>
        {
            StatusCode = HttpStatusCode.OK,
            Message = $"Got {logs.Count} controller logs for user '{id}'",
            Data = logs
        };
    }
}