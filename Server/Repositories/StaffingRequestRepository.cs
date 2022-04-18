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

public class StaffingRequestRepository : IStaffingRequestRepository
{
    private readonly DatabaseContext _context;
    private readonly ILoggingService _loggingService;
    private readonly INotificationRepository _notificationRepository;
    private readonly IConfiguration _configuration;

    public StaffingRequestRepository(DatabaseContext context, ILoggingService loggingService,
        INotificationRepository notificationRepository, IConfiguration configuration)
    {
        _context = context;
        _loggingService = loggingService;
        _notificationRepository = notificationRepository;
        _configuration = configuration;
    }

    #region Create

    /// <inheritdoc />
    public async Task<Response<string>> CreateStaffingRequest(StaffingRequest staffingRequest, HttpRequest request)
    {
        var result = await _context.StaffingRequests.AddAsync(staffingRequest);
        await _context.SaveChangesAsync();
        var newData = JsonConvert.SerializeObject(result.Entity);

        await _loggingService.AddWebsiteLog(request, $"Created staffing request '{result.Entity.Id}'", string.Empty, newData);

        var website = _configuration.GetSection("EmailOptions").GetValue<string>("website");
        await _notificationRepository.AddSeniorStaffNotification("New Staffing Request", $"A new staffing request has been submitted",
            $"{website}/staffingrequests/{result.Entity.Id}");

        return new Response<string>
        {
            StatusCode = HttpStatusCode.Created,
            Message = $"Created staffing request '{result.Entity.Id}'",
            Data = $"Created staffing request '{result.Entity.Id}'"
        };
    }

    #endregion

    #region Read

    /// <inheritdoc />
    public async Task<ResponsePaging<IList<StaffingRequest>>> GetStaffingRequests(int skip, int take, StaffingRequestStatus status)
    {
        var result = await _context.StaffingRequests
            .Where(x => x.Status == status)
            .Skip(skip).Take(take)
            .ToListAsync();
        var totalCount = await _context.StaffingRequests
            .Where(x => x.Status == status)
            .CountAsync();

        return new ResponsePaging<IList<StaffingRequest>>
        {
            StatusCode = HttpStatusCode.OK,
            TotalCount = totalCount,
            ResultCount = result.Count,
            Message = $"Got {result.Count} staffing requests",
            Data = result
        };
    }

    /// <inheritdoc />
    public async Task<Response<StaffingRequest>> GetStaffingRequest(int staffingRequestId)
    {
        var result = await _context.StaffingRequests.FindAsync(staffingRequestId) ??
            throw new StaffingRequestNotFoundException($"Staffing request '{staffingRequestId}' not found");
        return new Response<StaffingRequest>
        {
            StatusCode = HttpStatusCode.OK,
            Message = $"Got staffing request '{staffingRequestId}'",
            Data = result
        };
    }

    #endregion

    #region Update

    /// <inheritdoc />
    public async Task<Response<StaffingRequest>> UpdateStaffingRequest(StaffingRequest staffingRequest, HttpRequest request)
    {
        var dbStaffingRequest = await _context.StaffingRequests.AsNoTracking().FirstOrDefaultAsync(x => x.Id == staffingRequest.Id) ??
            throw new StaffingRequestNotFoundException($"Staffing request '{staffingRequest.Id}' not found");

        var oldData = JsonConvert.SerializeObject(dbStaffingRequest);
        var result = _context.StaffingRequests.Update(staffingRequest);
        await _context.SaveChangesAsync();
        var newData = JsonConvert.SerializeObject(result.Entity);

        await _loggingService.AddWebsiteLog(request, $"Updated staffing request '{result.Entity.Id}'", oldData, newData);

        return new Response<StaffingRequest>
        {
            StatusCode = HttpStatusCode.OK,
            Message = $"Updated staffing request '{result.Entity.Id}'",
            Data = result.Entity
        };
    }

    #endregion

    #region Delete

    /// <inheritdoc />
    public async Task<Response<StaffingRequest>> DeleteStaffingRequest(int staffingRequestId, HttpRequest request)
    {
        var staffingRequest = await _context.StaffingRequests.FindAsync(staffingRequestId) ??
            throw new StaffingRequestNotFoundException($"Staffing request '{staffingRequestId}' not found");

        _context.StaffingRequests.Remove(staffingRequest);
        await _context.SaveChangesAsync();
        var newData = JsonConvert.SerializeObject(staffingRequest);

        await _loggingService.AddWebsiteLog(request, $"Deleted staffing request '{staffingRequestId}'", string.Empty, newData);

        return new Response<StaffingRequest>
        {
            StatusCode = HttpStatusCode.OK,
            Message = $"Deleted staffing request '{staffingRequestId}'",
            Data = staffingRequest
        };
    }

    #endregion
}
