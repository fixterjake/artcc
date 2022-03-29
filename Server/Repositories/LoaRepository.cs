using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Serilog;
using System.Net;
using ZDC.Server.Data;
using ZDC.Server.Repositories.Interfaces;
using ZDC.Server.Services.Interfaces;
using ZDC.Shared;
using ZDC.Shared.Dtos;
using ZDC.Shared.Models;

namespace ZDC.Server.Repositories;

public class LoaRepository : ILoaRepository
{
    private readonly DatabaseContext _context;
    private readonly ILoggingService _loggingService;

    public LoaRepository(DatabaseContext context, ILoggingService loggingService)
    {
        _context = context;
        _loggingService = loggingService;
    }

    #region Create

    public async Task<Response<Loa>> CreateLoa(Loa loa, HttpRequest request)
    {
        var result = await _context.Loas.AddAsync(loa);
        await _context.SaveChangesAsync();
        var newData = JsonConvert.SerializeObject(result.Entity);

        await _loggingService.AddWebsiteLog(request, $"Created loa '{result.Entity.Id}'", string.Empty, newData);

        return new Response<Loa>
        {
            StatusCode = HttpStatusCode.Created,
            Message = $"Created loa '{result.Entity.Id}'",
            Data = result.Entity
        };
    }

    #endregion

    #region Read

    public async Task<Response<IList<Loa>>> GetLoas()
    {
        var loas = await _context.Loas.ToListAsync();
        return new Response<IList<Loa>>
        {
            StatusCode = HttpStatusCode.OK,
            Message = $"Got {loas.Count} loas",
            Data = loas
        };
    }

    public async Task<Response<Loa>> GetLoa(int loaId)
    {
        var loa = await _context.Loas
            .Include(x => x.User)
            .FirstOrDefaultAsync(x => x.Id == loaId) ??
            throw new LoaNotFoundException($"Loa '{loaId}' not found");

        return new Response<Loa>
        {
            StatusCode = HttpStatusCode.OK,
            Message = $"Got loa {loaId}''",
            Data = loa
        };
    }

    #endregion

    #region Update

    public async Task<Response<Loa>> UpdateLoa(Loa loa, HttpRequest request)
    {
        var dbLoa = await _context.Loas.AsNoTracking().FirstOrDefaultAsync(x => x.Id == loa.Id) ??
            throw new LoaNotFoundException($"Loa '{loa.Id}' not found");

        var oldData = JsonConvert.SerializeObject(dbLoa);
        _context.Loas.Update(loa);
        await _context.SaveChangesAsync();
        var newData = JsonConvert.SerializeObject(loa);

        await _loggingService.AddWebsiteLog(request, $"Updated loa '{loa.Id}'", oldData, newData);

        return new Response<Loa>
        {
            StatusCode = HttpStatusCode.Created,
            Message = $"Updated loa '{loa.Id}'",
            Data = loa
        };
    }


    #endregion

    #region Delete

    public async Task<Response<Loa>> DeleteLoa(int loaId, HttpRequest request)
    {
        var loa = await _context.Loas.FindAsync(loaId) ??
            throw new LoaNotFoundException($"Loa '{loaId}' not found");

        var oldData = JsonConvert.SerializeObject(loa);
        _context.Loas.Remove(loa);
        await _context.SaveChangesAsync();

        await _loggingService.AddWebsiteLog(request, $"Deleted loa '{loa.Id}'", oldData, string.Empty);

        return new Response<Loa>
        {
            StatusCode = HttpStatusCode.Created,
            Message = $"Deleted loa '{loa.Id}'",
            Data = loa
        };
    }

    #endregion
}
