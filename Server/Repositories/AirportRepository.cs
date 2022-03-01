﻿using System.Net;
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

    public async Task<Response<Airport>> GetAirport(int id)
    {
        var result = await _context.Airports.FindAsync(id) ??
                     throw new AirportNotFoundException($"Airport '{id}' not found");
        return new Response<Airport>
        {
            StatusCode = HttpStatusCode.OK,
            Message = $"Got airport '{id}'",
            Data = result
        };
    }

    public async Task<Response<Airport>> UpdateAirport(Airport airport, HttpRequest request)
    {
        var dbAirport = await _context.Airports.AsNoTracking().FirstOrDefaultAsync(x => x.Id == airport.Id) ??
                        throw new AirportNotFoundException($"Airport '{airport.Id}' not found");

        var oldData = JsonConvert.SerializeObject(dbAirport);
        var result = _context.Airports.Update(airport);
        await _context.SaveChangesAsync();
        var newData = JsonConvert.SerializeObject(result.Entity);

        await _loggingService.AddWebsiteLog(request, $"Updated airport '{result.Entity.Id}'", oldData, newData);

        return new Response<Airport>
        {
            StatusCode = HttpStatusCode.Created,
            Message = $"Updated airport '{result.Entity.Id}'",
            Data = result.Entity
        };
    }

    public async Task<Response<Airport>> DeleteAirport(int id, HttpRequest request)
    {
        var result = await _context.Airports.FindAsync(id) ??
                     throw new AirportNotFoundException($"Airport '{id}' not found");

        var oldData = JsonConvert.SerializeObject(result);
        _context.Airports.Remove(result);

        await _loggingService.AddWebsiteLog(request, $"Deleted airport '{id}'", oldData, string.Empty);

        return new Response<Airport>
        {
            StatusCode = HttpStatusCode.Created,
            Message = $"Deleted airport '{id}'",
            Data = result
        };
    }
}