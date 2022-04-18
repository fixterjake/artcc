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
    private readonly INotificationRepository _notificationRepository;
    private readonly IConfiguration _configuration;

    public LoaRepository(DatabaseContext context, ILoggingService loggingService,
        INotificationRepository notificationRepository, IConfiguration configuration)
    {
        _context = context;
        _loggingService = loggingService;
        _notificationRepository = notificationRepository;
        _configuration = configuration;
    }

    #region Create

    /// <inheritdoc />
    public async Task<Response<Loa>> CreateLoa(Loa loa, HttpRequest request)
    {
        var result = await _context.Loas.AddAsync(loa);
        await _context.SaveChangesAsync();
        var newData = JsonConvert.SerializeObject(result.Entity);

        await _loggingService.AddWebsiteLog(request, $"Created loa '{result.Entity.Id}'", string.Empty, newData);

        var website = _configuration.GetSection("EmailOptions").GetValue<string>("website");
        await _notificationRepository.AddSeniorStaffNotification("New LOA Request", $"A new LOA request has been submitted",
            $"{website}/loas/{result.Entity.Id}");

        return new Response<Loa>
        {
            StatusCode = HttpStatusCode.Created,
            Message = $"Created loa '{result.Entity.Id}'",
            Data = result.Entity
        };
    }

    #endregion

    #region Read

    /// <inheritdoc />
    public async Task<ResponsePaging<IList<Loa>>> GetLoas(int skip, int take)
    {
        var result = await _context.Loas
            .Skip(skip).Take(take)
            .Include(x => x.User)
            .ToListAsync();
        var totalCount = await _context.Loas.CountAsync();
        return new ResponsePaging<IList<Loa>>
        {
            StatusCode = HttpStatusCode.OK,
            TotalCount = totalCount,
            ResultCount = result.Count,
            Message = $"Got {result.Count} loas",
            Data = result
        };
    }

    /// <inheritdoc />
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

    /// <inheritdoc />
    public async Task<Response<Loa>> UpdateLoa(Loa loa, HttpRequest request)
    {
        var dbLoa = await _context.Loas.AsNoTracking().FirstOrDefaultAsync(x => x.Id == loa.Id) ??
            throw new LoaNotFoundException($"Loa '{loa.Id}' not found");

        var oldData = JsonConvert.SerializeObject(dbLoa);
        loa.Updated = DateTimeOffset.UtcNow;
        var result = _context.Loas.Update(loa);
        await _context.SaveChangesAsync();
        var newData = JsonConvert.SerializeObject(result.Entity);

        await _loggingService.AddWebsiteLog(request, $"Updated loa '{result.Entity.Id}'", oldData, newData);

        return new Response<Loa>
        {
            StatusCode = HttpStatusCode.Created,
            Message = $"Updated loa '{result.Entity.Id}'",
            Data = result.Entity
        };
    }


    #endregion

    #region Delete

    /// <inheritdoc />
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
