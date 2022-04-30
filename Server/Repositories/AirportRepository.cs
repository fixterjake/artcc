using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using System.Net;
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
    private readonly IDistributedCache _cache;
    private readonly ILoggingService _loggingService;

    public AirportRepository(DatabaseContext context, IDistributedCache cache, ILoggingService loggingService)
    {
        _context = context;
        _cache = cache;
        _loggingService = loggingService;
    }

    #region Create

    /// <inheritdoc />
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

    /// <inheritdoc />
    public async Task<Response<IList<Airport>>> GetAirports()
    {
        var cachedAirports = await _cache.GetStringAsync("_airports");
        if (!string.IsNullOrEmpty(cachedAirports))
        {
            var airports = JsonConvert.DeserializeObject<List<Airport>>(cachedAirports);
            if (airports != null)
                return new Response<IList<Airport>>
                {
                    StatusCode = HttpStatusCode.OK,
                    Message = $"Got {airports.Count} airports",
                    Data = airports
                };
        }

        var dbAirports = await _context.Airports.ToListAsync();
        var expiryOptions = new DistributedCacheEntryOptions()
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(2),
            SlidingExpiration = TimeSpan.FromMinutes(1)
        };
        await _cache.SetStringAsync("_airports", JsonConvert.SerializeObject(dbAirports), expiryOptions);

        return new Response<IList<Airport>>
        {
            StatusCode = HttpStatusCode.OK,
            Message = $"Got {dbAirports.Count} airports",
            Data = dbAirports
        };
    }

    /// <inheritdoc />
    public async Task<Response<Airport>> GetAirport(int airportId)
    {
        var airport = await _context.Airports.FindAsync(airportId) ??
            throw new AirportNotFoundException($"Airport '{airportId}' not found");
        return new Response<Airport>
        {
            StatusCode = HttpStatusCode.OK,
            Message = $"Got airport '{airportId}'",
            Data = airport
        };
    }

    #endregion

    #region Update

    /// <inheritdoc />
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

    /// <inheritdoc />
    public async Task<Response<Airport>> DeleteAirport(int airportId, HttpRequest request)
    {
        var airport = await _context.Airports.FindAsync(airportId) ??
            throw new AirportNotFoundException($"Airport '{airportId}' not found");

        var oldData = JsonConvert.SerializeObject(airport);
        _context.Airports.Remove(airport);

        await _loggingService.AddWebsiteLog(request, $"Deleted airport '{airportId}'", oldData, string.Empty);

        return new Response<Airport>
        {
            StatusCode = HttpStatusCode.Created,
            Message = $"Deleted airport '{airportId}'",
            Data = airport
        };
    }

    #endregion
}