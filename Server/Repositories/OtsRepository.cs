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

public class OtsRepository : IOtsRepository
{
    private readonly DatabaseContext _context;
    private readonly ILoggingService _loggingService;
    private readonly INotificationRepository _notificationRepository;
    private readonly IConfiguration _configuration;

    public OtsRepository(DatabaseContext context, ILoggingService loggingService,
        INotificationRepository notificationRepository, IConfiguration configuration)
    {
        _context = context;
        _loggingService = loggingService;
        _notificationRepository = notificationRepository;
        _configuration = configuration;
    }

    #region Create

    /// <inheritdoc />
    public async Task<Response<Ots>> CreateOts(Ots ots, HttpRequest request)
    {
        if (await _context.Users.FindAsync(ots.UserId) == null)
            throw new UserNotFoundException($"User '{ots.UserId}' not found");
        if (ots.InstructorId != null && await _context.Users.FindAsync(ots.InstructorId) == null)
            throw new UserNotFoundException($"User '{ots.InstructorId}' not found");
        if (await _context.Users.FindAsync(ots.RecommenderId) == null)
            throw new UserNotFoundException($"User '{ots.RecommenderId}' not found");

        var result = await _context.Ots.AddAsync(ots);
        await _context.SaveChangesAsync();
        var newData = JsonConvert.SerializeObject(result.Entity);

        await _loggingService.AddWebsiteLog(request, $"Created ots '{result.Entity.Id}'", string.Empty, newData);

        var website = _configuration.GetSection("EmailOptions").GetValue<string>("website");
        await _notificationRepository.AddTrainingNotification("New OTS", $"An OTS has been submitted",
            $"{website}/ots/{result.Entity.Id}");

        return new Response<Ots>
        {
            StatusCode = HttpStatusCode.Created,
            Message = $"Created ots '{result.Entity.Id}'",
            Data = result.Entity
        };
    }

    #endregion

    #region Read

    /// <inheritdoc />
    public async Task<ResponsePaging<IList<Ots>>> GetOts(int skip, int take, OtsStatus status)
    {
        var ots = await _context.Ots
            .Where(x => x.Status == status)
            .Skip(skip).Take(take)
            .Include(x => x.User)
            .Include(x => x.Instructor)
            .Include(x => x.Recommender)
            .OrderBy(x => x.Updated)
            .ToListAsync();
        var totalCount = await _context.Ots
            .Where(x => x.Status == status)
            .CountAsync();

        return new ResponsePaging<IList<Ots>>
        {
            StatusCode = HttpStatusCode.OK,
            TotalCount = totalCount,
            ResultCount = ots.Count,
            Message = $"Got {ots.Count} ots's",
            Data = ots
        };
    }

    /// <inheritdoc />
    public async Task<Response<Ots>> GetOts(int otsId)
    {
        var ots = await _context.Ots
            .Include(x => x.User)
            .Include(x => x.Instructor)
            .Include(x => x.Recommender)
            .FirstOrDefaultAsync(x => x.Id == otsId) ??
            throw new OtsNotFoundException($"OTS '{otsId}' not found");

        return new Response<Ots>
        {
            StatusCode = HttpStatusCode.OK,
            Message = $"Got OTS '{otsId}'",
            Data = ots
        };
    }

    #endregion

    #region Update

    /// <inheritdoc />
    public async Task<Response<Ots>> UpdateOts(Ots ots, HttpRequest request)
    {
        if (await _context.Users.FindAsync(ots.UserId) == null)
            throw new UserNotFoundException($"User '{ots.UserId}' not found");
        if (ots.InstructorId != 0 && await _context.Users.FindAsync(ots.InstructorId) == null)
            throw new UserNotFoundException($"User '{ots.InstructorId}' not found");
        if (await _context.Users.FindAsync(ots.RecommenderId) == null)
            throw new UserNotFoundException($"User '{ots.RecommenderId}' not found");

        var dbOts = await _context.Ots.AsNoTracking().FirstOrDefaultAsync(x => x.Id == ots.Id) ??
            throw new OtsNotFoundException($"OTS '{ots.Id}' not found");

        var oldData = JsonConvert.SerializeObject(dbOts);
        ots.Updated = DateTimeOffset.UtcNow;
        var result = _context.Ots.Update(ots);
        await _context.SaveChangesAsync();
        var newData = JsonConvert.SerializeObject(result.Entity);

        await _loggingService.AddWebsiteLog(request, $"Updated ots '{result.Entity.Id}'", oldData, newData);

        return new Response<Ots>
        {
            StatusCode = HttpStatusCode.Created,
            Message = $"Updated ots '{result.Entity.Id}'",
            Data = result.Entity
        };
    }

    #endregion

    #region Delete

    /// <inheritdoc />
    public async Task<Response<Ots>> DeleteOts(int otsId, HttpRequest request)
    {
        var ots = await _context.Ots.FindAsync(otsId) ??
            throw new OtsNotFoundException($"OTS '{otsId}' not found");

        var oldData = JsonConvert.SerializeObject(ots);
        _context.Ots.Remove(ots);
        await _context.SaveChangesAsync();

        await _loggingService.AddWebsiteLog(request, $"Deleted ots '{otsId}'", oldData, string.Empty);

        return new Response<Ots>
        {
            StatusCode = HttpStatusCode.Created,
            Message = $"Deleted ots '{otsId}'",
            Data = ots
        };
    }

    #endregion
}
