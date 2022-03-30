using Microsoft.EntityFrameworkCore;
using System.Net;
using ZDC.Server.Data;
using ZDC.Server.Extensions;
using ZDC.Server.Repositories.Interfaces;
using ZDC.Shared;
using ZDC.Shared.Dtos;
using ZDC.Shared.Models;

namespace ZDC.Server.Repositories;

public class NotificationRepository : INotificationRepository
{
    private readonly DatabaseContext _context;

    public NotificationRepository(DatabaseContext context)
    {
        _context = context;
    }

    public async Task AddNotification(Notification notification)
    {
        await _context.Notifications.AddAsync(notification);
        await _context.SaveChangesAsync();
    }

    public async Task<Response<IList<Notification>>> GetNotifications(HttpRequest request)
    {
        var user = await request.HttpContext.GetUser(_context) ??
            throw new UserNotFoundException("User not found");

        var notifications = await _context.Notifications
            .Where(x => !x.Read)
            .Where(x => x.UserId == user.Id)
            .ToListAsync();

        return new Response<IList<Notification>>
        {
            StatusCode = HttpStatusCode.OK,
            Message = $"Got {notifications.Count} notifications",
            Data = notifications
        };
    }

    public async Task ReadNotification(int notificationId, HttpRequest request)
    {
        var notification = await _context.Notifications.FindAsync(notificationId) ??
            throw new NotificationNotFoundException($"Notification '{notificationId}' not found");
        notification.Read = true;
        await _context.SaveChangesAsync();
    }
}
