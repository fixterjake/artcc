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

public class FeedbackRepository : IFeedbackRepository
{
    private readonly DatabaseContext _context;
    private readonly IDistributedCache _cache;
    private readonly ILoggingService _loggingService;
    private readonly INotificationRepository _notificationRepository;
    private readonly IConfiguration _configuration;

    public FeedbackRepository(DatabaseContext context, IDistributedCache cache, ILoggingService loggingService,
        INotificationRepository notificationRepository, IConfiguration configuration)
    {
        _context = context;
        _cache = cache;
        _loggingService = loggingService;
        _notificationRepository = notificationRepository;
        _configuration = configuration;
    }

    #region Create

    public async Task<Response<string>> CreateFeedback(Feedback feedback, HttpRequest request)
    {
        var result = await _context.Feedback.AddAsync(feedback);
        await _context.SaveChangesAsync();
        var newData = JsonConvert.SerializeObject(result.Entity);

        await _loggingService.AddWebsiteLog(request, $"Created feedback '{result.Entity.Id}'", string.Empty, newData);

        var website = _configuration.GetSection("EmailOptions").GetValue<string>("website");
        await _notificationRepository.AddSeniorStaffNotification("New Feedback", $"New feedback has been submitted",
            $"{website}/feedback/{result.Entity.Id}");

        return new Response<string>
        {
            StatusCode = HttpStatusCode.Created,
            Message = $"Created feedback '{result.Entity.Id}'",
            Data = $"Created feedback '{result.Entity.Id}'"
        };
    }

    #endregion

    #region Read

    public async Task<Response<IList<Feedback>>> GetFeedback()
    {
        var cachedFeedback = await _cache.GetStringAsync("_feedback");
        if (!string.IsNullOrEmpty(cachedFeedback))
        {
            var feedback = JsonConvert.DeserializeObject<List<Feedback>>(cachedFeedback);
            if (feedback != null)
                return new Response<IList<Feedback>>
                {
                    StatusCode = HttpStatusCode.OK,
                    Message = $"Got {feedback.Count} feedback",
                    Data = feedback
                };
        }

        var result = await _context.Feedback.ToListAsync();
        var expiryOptions = new DistributedCacheEntryOptions()
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(2),
            SlidingExpiration = TimeSpan.FromMinutes(1)
        };
        await _cache.SetStringAsync("_feedback", JsonConvert.SerializeObject(result), expiryOptions);

        return new Response<IList<Feedback>>
        {
            StatusCode = HttpStatusCode.OK,
            Message = $"Got {result.Count} feedback",
            Data = result
        };
    }

    public async Task<Response<Feedback>> GetFeedback(int feedbackId)
    {
        var result = await _context.Feedback
            .Include(x => x.User)
            .FirstOrDefaultAsync(x => x.Id == feedbackId) ??
            throw new FeedbackNotFoundException($"Feedback '{feedbackId}' not found");

        return new Response<Feedback>
        {
            StatusCode = HttpStatusCode.OK,
            Message = $"Got feedback '{feedbackId}'",
            Data = result
        };
    }

    #endregion

    #region Update

    public async Task<Response<Feedback>> UpdateFeedback(Feedback feedback, HttpRequest request)
    {
        var dbFeedback = await _context.Feedback.AsNoTracking().FirstOrDefaultAsync(x => x.Id == feedback.Id) ??
            throw new FeedbackNotFoundException($"Feedback '{feedback.Id}' not found");

        var oldData = JsonConvert.SerializeObject(dbFeedback);
        var result = _context.Feedback.Update(feedback);
        await _context.SaveChangesAsync();
        var newData = JsonConvert.SerializeObject(result.Entity);

        await _loggingService.AddWebsiteLog(request, $"Updated feedback '{result.Entity.Id}'", oldData, newData);

        return new Response<Feedback>
        {
            StatusCode = HttpStatusCode.OK,
            Message = $"Updated feedback '{result.Entity.Id}'",
            Data = result.Entity
        };
    }

    #endregion

    #region Delete

    public async Task<Response<Feedback>> DeleteFeedback(int feedbackId, HttpRequest request)
    {
        var feedback = await _context.Feedback.FindAsync(feedbackId) ??
            throw new FeedbackNotFoundException($"Feedback '{feedbackId}' not found");

        _context.Feedback.Remove(feedback);
        await _context.SaveChangesAsync();
        var newData = JsonConvert.SerializeObject(feedback);

        await _loggingService.AddWebsiteLog(request, $"Deleted feedback '{feedback.Id}'", string.Empty, newData);

        return new Response<Feedback>
        {
            StatusCode = HttpStatusCode.OK,
            Message = $"Deleted feedback '{feedback.Id}'",
            Data = feedback
        };
    }

    #endregion
}
