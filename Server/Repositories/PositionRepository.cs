using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Net;
using ZDC.Server.Data;
using ZDC.Server.Repositories.Interfaces;
using ZDC.Server.Services.Interfaces;
using ZDC.Shared;
using ZDC.Shared.Dtos;
using ZDC.Shared.Models;

namespace ZDC.Server.Repositories;

public class PositionRepository : IPositionRepository
{
    private readonly DatabaseContext _context;
    private readonly ILoggingService _loggingService;

    public PositionRepository(DatabaseContext context, ILoggingService loggingService)
    {
        _context = context;
        _loggingService = loggingService;
    }

    #region Create

    public async Task<Response<Position>> CreatePosition(Position position, HttpRequest request)
    {
        var result = await _context.Positions.AddAsync(position);
        await _context.SaveChangesAsync();
        var newData = JsonConvert.SerializeObject(result.Entity);

        await _loggingService.AddWebsiteLog(request, $"Created position '{result.Entity.Id}'", string.Empty, newData);

        return new Response<Position>
        {
            StatusCode = HttpStatusCode.Created,
            Message = $"Created position '{result.Entity.Id}'",
            Data = result.Entity
        };
    }

    #endregion

    #region Read

    public async Task<Response<IList<Position>>> GetPositions()
    {
        var positions = await _context.Positions.ToListAsync();
        return new Response<IList<Position>>
        {
            StatusCode = HttpStatusCode.OK,
            Message = $"Got {positions.Count} positions",
            Data = positions
        };
    }

    #endregion

    #region Delete

    public async Task<Response<Position>> DeletePosition(int positionId, HttpRequest request)
    {
        var position = await _context.Positions.FindAsync(positionId) ??
            throw new PositionNotFoundException($"Position '{positionId}' not found");

        var oldData = JsonConvert.SerializeObject(position);
        _context.Positions.Remove(position);
        await _context.SaveChangesAsync();

        await _loggingService.AddWebsiteLog(request, $"Deleted position '{positionId}'", oldData, string.Empty);

        return new Response<Position>
        {
            StatusCode = HttpStatusCode.Created,
            Message = $"Deleted position '{positionId}'",
            Data = position
        };
    }

    #endregion
}
