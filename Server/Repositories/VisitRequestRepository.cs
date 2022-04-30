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

public class VisitRequestRepository : IVisitRequestRepository
{
    private readonly DatabaseContext _context;
    private readonly INotificationRepository _notificationRepository;
    private readonly ILoggingService _loggingService;
    private readonly IVatsimService _vatsimService;
    private readonly IConfiguration _configuration;

    public VisitRequestRepository(DatabaseContext context, INotificationRepository notificationRepository, ILoggingService loggingService,
        IVatsimService vatsimService, IConfiguration configuration)
    {
        _context = context;
        _notificationRepository = notificationRepository;
        _loggingService = loggingService;
        _vatsimService = vatsimService;
        _configuration = configuration;
    }

    #region Create

    /// <inheritdoc />
    public async Task<Response<VisitRequest>> CreateVisitRequest(VisitRequest visitRequest, HttpRequest request)
    {
        var ratingHours = await _vatsimService.GetUserRatingHours(visitRequest.Cid, visitRequest.Rating);
        visitRequest.RatingHours = ratingHours;
        var lastRatingChange = await _vatsimService.GetLastRatingChange(visitRequest.Cid);
        visitRequest.LastRatingChange = DateTime.SpecifyKind(lastRatingChange, DateTimeKind.Utc);

        var result = await _context.VisitRequests.AddAsync(visitRequest);
        await _context.SaveChangesAsync();
        var newData = JsonConvert.SerializeObject(result.Entity);

        await _loggingService.AddWebsiteLog(request, $"Created visit request '{result.Entity.Id}'", string.Empty, newData);

        var website = _configuration.GetSection("EmailOptions").GetValue<string>("website");
        await _notificationRepository.AddSeniorStaffNotification("New Visit Request", "A new visit request has been submitted",
            $"{website}/visitrequests/{result.Entity.Id}");

        return new Response<VisitRequest>
        {
            StatusCode = HttpStatusCode.Created,
            Message = $"Created visit request '{result.Entity.Id}'",
            Data = result.Entity
        };
    }

    #endregion

    #region Read

    /// <inheritdoc />
    public async Task<ResponsePaging<IList<VisitRequest>>> GetVisitRequests(int skip, int take, VisitRequestStatus status)
    {
        var visitRequests = await _context.VisitRequests
            .Where(x => x.Status == status)
            .Skip(skip).Take(take)
            .ToListAsync();
        var totalCount = await _context.VisitRequests.CountAsync();

        return new ResponsePaging<IList<VisitRequest>>
        {
            StatusCode = HttpStatusCode.OK,
            TotalCount = totalCount,
            ResultCount = visitRequests.Count,
            Message = $"Got {visitRequests.Count} visit requests",
            Data = visitRequests
        };
    }

    /// <inheritdoc />
    public async Task<Response<VisitRequest>> GetVisitRequest(int visitRequestId)
    {
        var result = await _context.VisitRequests.FindAsync(visitRequestId) ??
            throw new VisitRequestNotFoundException($"Visit request '{visitRequestId}' not found");

        return new Response<VisitRequest>
        {
            StatusCode = HttpStatusCode.OK,
            Message = $"Got visit request '{visitRequestId}'",
            Data = result
        };
    }

    #endregion

    #region Update

    /// <inheritdoc />
    public async Task<Response<VisitRequest>> UpdateVisitRequest(VisitRequest visitRequest, HttpRequest request)
    {
        var dbResult = await _context.VisitRequests.AsNoTracking().FirstOrDefaultAsync(x => x.Id == visitRequest.Id) ??
            throw new VisitRequestNotFoundException($"Visit request '{visitRequest.Id}' not found");

        var oldData = JsonConvert.SerializeObject(dbResult);
        var result = _context.VisitRequests.Update(visitRequest);
        await _context.SaveChangesAsync();
        var newData = JsonConvert.SerializeObject(result.Entity);

        await _loggingService.AddWebsiteLog(request, $"Updated visit request '{result.Entity.Id}'", oldData, newData);

        return new Response<VisitRequest>
        {
            StatusCode = HttpStatusCode.OK,
            Message = $"Updated visit request '{result.Entity.Id}'",
            Data = result.Entity
        };
    }

    #endregion

    #region Delete

    /// <inheritdoc />
    public async Task<Response<VisitRequest>> DeleteVisitRequest(int visitRequestId, HttpRequest request)
    {
        var visitRequest = await _context.VisitRequests.FindAsync(visitRequestId) ??
            throw new VisitRequestNotFoundException($"Visit request '{visitRequestId}' not found");

        var oldData = JsonConvert.SerializeObject(visitRequest);
        _context.VisitRequests.Remove(visitRequest);

        await _loggingService.AddWebsiteLog(request, $"Deleted visit request '{visitRequestId}'", oldData, string.Empty);

        return new Response<VisitRequest>
        {
            StatusCode = HttpStatusCode.OK,
            Message = $"Deleted visit request '{visitRequestId}'",
            Data = visitRequest
        };
    }

    #endregion
}
