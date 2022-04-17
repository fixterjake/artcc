using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using System.Net;
using ZDC.Server.Data;
using ZDC.Server.Extensions;
using ZDC.Server.Repositories.Interfaces;
using ZDC.Server.Services.Interfaces;
using ZDC.Shared;
using ZDC.Shared.Dtos;
using ZDC.Shared.Models;

namespace ZDC.Server.Repositories;

public class AnnouncementRepository : IAnnouncementRepository
{
    private readonly DatabaseContext _context;
    private readonly IDistributedCache _cache;
    private readonly ILoggingService _loggingService;
    private readonly INotificationRepository _notificationRepository;
    private readonly IConfiguration _configuration;

    public AnnouncementRepository(DatabaseContext context, IDistributedCache cache, ILoggingService loggingService,
        INotificationRepository notificationRepository, IConfiguration configuration)
    {
        _context = context;
        _cache = cache;
        _loggingService = loggingService;
        _notificationRepository = notificationRepository;
        _configuration = configuration;
    }

    #region Create

    public async Task<Response<Announcement>> CreateAnnouncement(Announcement announcement, HttpRequest request)
    {
        var user = await request.HttpContext.GetUser(_context) ??
            throw new UserNotFoundException("User not found");

        announcement.UserId = user.Id;
        announcement.User = user.FullName;

        var result = await _context.Announcements.AddAsync(announcement);
        await _context.SaveChangesAsync();
        var newData = JsonConvert.SerializeObject(result.Entity);

        await _loggingService.AddWebsiteLog(request, $"Created announcement '{result.Entity.Id}'", string.Empty, newData);

        var website = _configuration.GetSection("EmailOptions").GetValue<string>("website");
        await _notificationRepository.AddSeniorStaffNotification("New Announcement", announcement.Title, $"{website}/announcements/{announcement.Id}");

        return new Response<Announcement>
        {
            StatusCode = HttpStatusCode.Created,
            Message = $"Created news '{result.Entity.Id}'",
            Data = result.Entity
        };
    }

    #endregion

    #region Read

    public async Task<Response<IList<Announcement>>> GetAnnouncements()
    {

        var cachedAnnouncements = await _cache.GetStringAsync("_announcements");
        if (!string.IsNullOrEmpty(cachedAnnouncements))
        {
            var announcements = JsonConvert.DeserializeObject<IList<Announcement>>(cachedAnnouncements);
            if (announcements != null)
                return new Response<IList<Announcement>>
                {
                    StatusCode = HttpStatusCode.OK,
                    Message = $"Got {announcements.Count} announcements",
                    Data = announcements
                };
        }

        var result = await _context.Announcements.ToListAsync();
        var expiryOptions = new DistributedCacheEntryOptions()
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(2),
            SlidingExpiration = TimeSpan.FromMinutes(1)
        };
        await _cache.SetStringAsync("_announcements", JsonConvert.SerializeObject(result), expiryOptions);

        return new Response<IList<Announcement>>
        {
            StatusCode = HttpStatusCode.OK,
            Message = $"Got {result.Count} announcements",
            Data = result
        };
    }

    public async Task<Response<Announcement>> GetAnnouncement(int announcementId)
    {
        var result = await _context.Announcements.FindAsync(announcementId) ??
            throw new AnnouncementNotFoundException($"Announcement '{announcementId}' not found");
        return new Response<Announcement>
        {
            StatusCode = HttpStatusCode.OK,
            Message = $"Got announcement '{result.Id}'",
            Data = result
        };
    }

    #endregion

    #region Update

    public async Task<Response<Announcement>> UpdateAnnouncement(Announcement announcement, HttpRequest request)
    {
        var user = await request.HttpContext.GetUser(_context) ??
            throw new UserNotFoundException("User not found");
        var dbAnnouncement = await _context.Announcements.FindAsync(announcement.Id) ??
            throw new AnnouncementNotFoundException($"Announcement '{announcement.Id}' not found");

        var oldData = JsonConvert.SerializeObject(dbAnnouncement);
        announcement.Updated = DateTimeOffset.UtcNow;
        announcement.User = user.FullName;
        var result = _context.Announcements.Update(announcement);
        await _context.SaveChangesAsync();
        var newData = JsonConvert.SerializeObject(result.Entity);

        await _loggingService.AddWebsiteLog(request, $"Updated announcement '{result.Entity.Id}'", oldData, newData);

        return new Response<Announcement>
        {
            StatusCode = HttpStatusCode.OK,
            Message = $"Updated announcement '{result.Entity.Id}'",
            Data = result.Entity
        };
    }

    #endregion

    #region Delete

    public async Task<Response<Announcement>> DeleteAnnouncement(int announcementId, HttpRequest request)
    {
        var announcement = await _context.Announcements.FindAsync(announcementId) ??
            throw new AnnouncementNotFoundException($"Announcement '{announcementId}' not found");

        var oldData = JsonConvert.SerializeObject(announcement);
        _context.Announcements.Remove(announcement);
        await _context.SaveChangesAsync();

        await _loggingService.AddWebsiteLog(request, $"Deleted announcement '{announcementId}'", oldData, string.Empty);

        return new Response<Announcement>
        {
            StatusCode = HttpStatusCode.OK,
            Message = $"Deleted announcement '{announcementId}'",
            Data = announcement
        };
    }

    #endregion
}
