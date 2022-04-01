using Microsoft.EntityFrameworkCore;
using ZDC.Server.Data;
using ZDC.Server.Repositories.Interfaces;
using ZDC.Shared.Dtos;
using ZDC.Shared.Models;

namespace ZDC.Server.Repositories;

public class OnlineControllerRepository : IOnlineControllerRepository
{
    private readonly DatabaseContext _context;

    public OnlineControllerRepository(DatabaseContext context)
    {
        _context = context;
    }

    public async Task<Response<IList<OnlineController>>> GetOnlineControllers()
    {
        var onlineControllers = await _context.OnlineControllers.ToListAsync();
        return new Response<IList<OnlineController>>
        {
            StatusCode = System.Net.HttpStatusCode.OK,
            Message = $"Got {onlineControllers.Count} online controllers",
            Data = onlineControllers
        };
    }
}
