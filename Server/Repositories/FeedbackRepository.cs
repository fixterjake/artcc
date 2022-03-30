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

public class FeedbackRepository : IFeedbackRepository
{
    private readonly DatabaseContext _context;
    private readonly ILoggingService _loggingService;
    private readonly INotificationRepository _notificationRepository;

    public FeedbackRepository(DatabaseContext context, ILoggingService loggingService, INotificationRepository notificationRepository)
    {
        _context = context;
        _loggingService = loggingService;
        _notificationRepository = notificationRepository;
    }

    #region Create

    public async Task<Response<string>> CreateFeedback(Feedback feedback, HttpRequest request)
    {
        var result = await _context.Feedback.AddAsync(feedback);
        await _context.SaveChangesAsync();
        var newData = JsonConvert.SerializeObject(result.Entity);

        await _loggingService.AddWebsiteLog(request, $"Created feedback '{result.Entity.Id}'", string.Empty, newData);



        return new Response<string>
        {
            StatusCode = HttpStatusCode.Created,
            Message = $"Created airport '{result.Entity.Id}'",
            Data = $"Created airport '{result.Entity.Id}'"
        };
    }

    #endregion

    #region Read

    public async Task<Response<IList<Feedback>>> GetFeedback()
    {
        var result = await _context.Feedback.ToListAsync();
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
