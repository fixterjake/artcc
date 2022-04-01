using System.Net;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using ZDC.Server.Data;
using ZDC.Server.Repositories.Interfaces;
using ZDC.Server.Services.Interfaces;
using ZDC.Shared;
using ZDC.Shared.Dtos;
using ZDC.Shared.Models;

namespace ZDC.Server.Repositories;

public class AirportRepository : IAirportRepository
{
    private readonly DatabaseContext _context;
    private readonly ILoggingService _loggingService;

    public AirportRepository(DatabaseContext context, ILoggingService loggingService)
    {
        _context = context;
        _loggingService = loggingService;
    }

    #region Create

    public async Task<Response<Airport>> CreateAirport(Airport airport, HttpRequest request)
    {
        var result = await _context.AddAsync(airport);
        await _context.SaveChangesAsync();
        var newData = JsonConvert.SerializeObject(result.Entity);

        await _loggingService.AddWebsiteLog(request, $"Created airport '{result.Entity.Id}'", string.Empty, newData);

        return new Response<Airport>
        {
            StatusCode = HttpStatusCode.Created,
            Message = $"Created airport '{result.Entity.Id}'",
            Data = result.Entity
        };
    }

    #endregion

    #region Read

    public async Task<Response<IList<Airport>>> GetAirports()
    {
        var result = await _context.Airports.ToListAsync();
        return new Response<IList<Airport>>
        {
            StatusCode = HttpStatusCode.OK,
            Message = $"Got {result.Count} airports",
            Data = result
        };
    }

    public async Task<Response<Airport>> GetAirport(int airportId)
    {
        var result = await _context.Airports.FindAsync(airportId) ??
            throw new AirportNotFoundException($"Airport '{airportId}' not found");
        return new Response<Airport>
        {
            StatusCode = HttpStatusCode.OK,
            Message = $"Got airport '{airportId}'",
            Data = result
        };
    }

    #endregion

    #region Update

    public async Task<Response<Airport>> UpdateAirport(Airport airport, HttpRequest request)
    {
        var dbAirport = await _context.Airports.AsNoTracking().FirstOrDefaultAsync(x => x.Id == airport.Id) ??
            throw new AirportNotFoundException($"Airport '{airport.Id}' not found");

        var oldData = JsonConvert.SerializeObject(dbAirport);
        airport.Updated = DateTimeOffset.UtcNow;
        var result = _context.Airports.Update(airport);
        await _context.SaveChangesAsync();
        var newData = JsonConvert.SerializeObject(result.Entity);

        await _loggingService.AddWebsiteLog(request, $"Updated airport '{result.Entity.Id}'", oldData, newData);

        return new Response<Airport>
        {
            StatusCode = HttpStatusCode.OK,
            Message = $"Updated airport '{result.Entity.Id}'",
            Data = result.Entity
        };
    }

    #endregion

    #region Delete

    public async Task<Response<Airport>> DeleteAirport(int airportId, HttpRequest request)
    {
        var result = await _context.Airports.FindAsync(airportId) ??
            throw new AirportNotFoundException($"Airport '{airportId}' not found");

        var oldData = JsonConvert.SerializeObject(result);
        _context.Airports.Remove(result);

        await _loggingService.AddWebsiteLog(request, $"Deleted airport '{airportId}'", oldData, string.Empty);

        return new Response<Airport>
        {
            StatusCode = HttpStatusCode.Created,
            Message = $"Deleted airport '{airportId}'",
            Data = result
        };
    }

    #endregion
}