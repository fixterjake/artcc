using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Net;
using ZDC.Server.Data;
using ZDC.Server.Repositories.Interfaces;
using ZDC.Server.Services.Interfaces;
using ZDC.Shared;
using ZDC.Shared.Dtos;
using ZDC.Shared.Models;

namespace ZDC.Server.Repositories;

public class WarningRepository : IWarningRepository
{
    private readonly DatabaseContext _context;
    private readonly ILoggingService _loggingService;

    public WarningRepository(DatabaseContext context, ILoggingService loggingService)
    {
        _context = context;
        _loggingService = loggingService;
    }

    #region Create

    /// <inheritdoc />
    public async Task<Response<Warning>> CreateWarning(Warning warning, HttpRequest request)
    {
        var result = await _context.Warnings.AddAsync(warning);
        await _context.SaveChangesAsync();
        var newData = JsonConvert.SerializeObject(result.Entity);

        await _loggingService.AddWebsiteLog(request, $"Created warning '{result.Entity.Id}'", string.Empty, newData);

        return new Response<Warning>
        {
            StatusCode = HttpStatusCode.Created,
            Message = $"Created warning '{result.Entity.Id}'",
            Data = result.Entity
        };
    }

    public async Task<Response<IList<Warning>>> CreateWarnings(IList<Warning> warnings, HttpRequest request)
    {
        await _context.Warnings.AddRangeAsync(warnings);
        await _context.SaveChangesAsync();

        foreach (var warning in warnings)
        {
            var newData = JsonConvert.SerializeObject(warning);
            await _loggingService.AddWebsiteLog(request, $"Created warning '{warning.Id}'", string.Empty, newData);
        }

        return new Response<IList<Warning>>
        {
            StatusCode = HttpStatusCode.Created,
            Message = $"Created {warnings.Count}' warnings",
            Data = warnings
        };
    }

    #endregion

    #region Read

    /// <inheritdoc />
    public async Task<Response<IList<AuditDto>>> AuditControllers()
    {
        var users = await _context.Users
            .Include(x => x.Roles)
            .Where(x => x.Status != UserStatus.Removed && x.Status != UserStatus.Exempt)
            .OrderBy(x => x.LastName)
            .ToListAsync();
        var result = new List<AuditDto>();

        foreach (var user in users)
        {
            var hours = await _context.Hours
                .Where(x => x.UserId == user.Id)
                .OrderByDescending(x => x.Year)
                .OrderByDescending(x => x.Month)
                .Take(6).ToListAsync();
            result.Add(new AuditDto
            {
                Month = DateTime.UtcNow.Month,
                Year = DateTime.UtcNow.Year,
                User = user,
                SixMonthHours = hours
            });
        }

        return new Response<IList<AuditDto>>
        {
            StatusCode = HttpStatusCode.OK,
            Message = $"Audited {result.Count} controllers",
            Data = result
        };
    }

    /// <inheritdoc />
    public async Task<Response<IList<Warning>>> GetWarnings(int month, int year)
    {
        var warnings = await _context.Warnings
            .Where(x => x.Month == month && x.Year == year)
            .ToListAsync();
        return new Response<IList<Warning>>
        {
            StatusCode = HttpStatusCode.OK,
            Message = $"Got {warnings.Count} warnings",
            Data = warnings
        };
    }

    #endregion

    #region Delete

    /// <inheritdoc />
    public async Task<Response<Warning>> DeleteWarning(int warningId, HttpRequest request)
    {
        var warning = await _context.Warnings.FindAsync(warningId) ??
            throw new WarningNotFoundException($"Warning '{warningId}' not found");

        var oldData = JsonConvert.SerializeObject(warning);
        _context.Warnings.Remove(warning);
        await _context.SaveChangesAsync();

        await _loggingService.AddWebsiteLog(request, $"Deleted warning '{warningId}'", oldData, string.Empty);

        return new Response<Warning>
        {
            StatusCode = HttpStatusCode.OK,
            Message = $"Deleted warning '{warningId}'",
            Data = warning
        };
    }

    #endregion
}
