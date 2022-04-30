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

    /// <inheritdoc />
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

    /// <inheritdoc />
    public async Task<ResponsePaging<IList<Announcement>>> GetAnnouncements(int skip, int take)
    {

        var cachedAnnouncements = await _cache.GetStringAsync($"_announcements_{skip}_{take}");
        var cachedCount = await _cache.GetStringAsync("_announcements_count");
        if (!string.IsNullOrEmpty(cachedAnnouncements))
        {
            var announcements = JsonConvert.DeserializeObject<IList<Announcement>>(cachedAnnouncements);
            var count = int.Parse(cachedCount);
            if (announcements != null)
                return new ResponsePaging<IList<Announcement>>
                {
                    StatusCode = HttpStatusCode.OK,
                    TotalCount = count,
                    ResultCount = announcements.Count,
                    Message = $"Got {announcements.Count} announcements",
                    Data = announcements
                };
        }

        var dbAnnouncements = await _context.Announcements
            .Skip(skip).Take(take)
            .ToListAsync();
        var totalCount = await _context.Announcements.CountAsync();
        var expiryOptions = new DistributedCacheEntryOptions()
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(2),
            SlidingExpiration = TimeSpan.FromMinutes(1)
        };
        await _cache.SetStringAsync($"_announcements_{skip}_{take}", JsonConvert.SerializeObject(dbAnnouncements), expiryOptions);
        await _cache.SetStringAsync("_announcements_count", $"{totalCount}", expiryOptions);

        return new ResponsePaging<IList<Announcement>>
        {
            StatusCode = HttpStatusCode.OK,
            TotalCount = totalCount,
            ResultCount = dbAnnouncements.Count,
            Message = $"Got {dbAnnouncements.Count} announcements",
            Data = dbAnnouncements
        };
    }

    /// <inheritdoc />
    public async Task<Response<Announcement>> GetAnnouncement(int announcementId)
    {
        var announcement = await _context.Announcements.FindAsync(announcementId) ??
            throw new AnnouncementNotFoundException($"Announcement '{announcementId}' not found");
        return new Response<Announcement>
        {
            StatusCode = HttpStatusCode.OK,
            Message = $"Got announcement '{announcement.Id}'",
            Data = announcement
        };
    }

    #endregion

    #region Update

    /// <inheritdoc />
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

    /// <inheritdoc />
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
