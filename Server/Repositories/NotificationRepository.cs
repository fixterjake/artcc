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

    #region Create

    /// <inheritdoc />
    public async Task AddSeniorStaffNotification(string title, string description, string link)
    {
        var users = await _context.Users
            .Include(x => x.Roles)
            .ToListAsync();
        var usersFinal = new List<User>();
        foreach (var entry in users)
        {
            if (entry.Roles == null || entry.Roles.Count == 0)
                continue;
            var roles = entry.Roles.Select(x => x.Name);
            if (roles.Contains("ATM") || roles.Contains("DATM") ||
                roles.Contains("TA") || roles.Contains("WM"))
                usersFinal.Add(entry);
        }
        foreach (var entry in usersFinal)
            await AddNotification(new Notification
            {
                UserId = entry.Id,
                Title = title,
                Description = description,
                Link = link
            });
    }

    /// <inheritdoc />
    public async Task AddStaffNotification(string title, string description, string link)
    {
        var users = await _context.Users
            .Include(x => x.Roles)
            .ToListAsync();
        var usersFinal = new List<User>();
        foreach (var entry in users)
        {
            if (entry.Roles == null || entry.Roles.Count == 0)
                continue;
            var roles = entry.Roles.Select(x => x.Name);
            if (roles.Contains("ATM") || roles.Contains("DATM") ||
                roles.Contains("TA") || roles.Contains("ATA") ||
                roles.Contains("WM") || roles.Contains("AWM") ||
                roles.Contains("EC") || roles.Contains("AEC") ||
                roles.Contains("FE") || roles.Contains("AFE"))
                usersFinal.Add(entry);
        }
        foreach (var entry in usersFinal)
            await AddNotification(new Notification
            {
                UserId = entry.Id,
                Title = title,
                Description = description,
                Link = link
            });
    }

    /// <inheritdoc />
    public async Task AddTrainingNotification(string title, string description, string link)
    {
        var users = await _context.Users
            .Include(x => x.Roles)
            .ToListAsync();
        var usersFinal = new List<User>();
        foreach (var entry in users)
        {
            if (entry.Roles == null || entry.Roles.Count == 0)
                continue;
            var roles = entry.Roles.Select(x => x.Name);
            if (roles.Contains("ATM") || roles.Contains("DATM") ||
                roles.Contains("TA") || roles.Contains("ATA") ||
                roles.Contains("WM"))
                usersFinal.Add(entry);
        }
        foreach (var entry in usersFinal)
            await AddNotification(new Notification
            {
                UserId = entry.Id,
                Title = title,
                Description = description,
                Link = link
            });
    }

    /// <inheritdoc />
    public async Task AddEventsNotification(string title, string description, string link)
    {
        var users = await _context.Users
            .Include(x => x.Roles)
            .ToListAsync();
        var usersFinal = new List<User>();
        foreach (var entry in users)
        {
            if (entry.Roles == null || entry.Roles.Count == 0)
                continue;
            var roles = entry.Roles.Select(x => x.Name);
            if (roles.Contains("ATM") || roles.Contains("DATM") ||
                roles.Contains("TA") || roles.Contains("WM") ||
                roles.Contains("EC") || roles.Contains("AEC") ||
                roles.Contains("EVENTS"))
                usersFinal.Add(entry);
        }
        foreach (var entry in usersFinal)
            await AddNotification(new Notification
            {
                UserId = entry.Id,
                Title = title,
                Description = description,
                Link = link
            });
    }

    /// <inheritdoc />
    public async Task AddAllNotification(string title, string description, string link)
    {
        var users = await _context.Users.Where(x => x.Status != UserStatus.Removed).ToListAsync();
        foreach (var entry in users)
            await AddNotification(new Notification
            {
                UserId = entry.Id,
                Title = title,
                Description = description,
                Link = link
            });
    }

    /// <inheritdoc />
    public async Task AddNotification(Notification notification)
    {
        await _context.Notifications.AddAsync(notification);
        await _context.SaveChangesAsync();
    }

    #endregion

    #region Read

    /// <inheritdoc />
    public async Task<ResponsePaging<IList<Notification>>> GetNotifications(int skip, int take, HttpRequest request)
    {
        var user = await request.HttpContext.GetUser(_context) ??
            throw new UserNotFoundException("User not found");

        var notifications = await _context.Notifications
            .Where(x => !x.Read)
            .Where(x => x.UserId == user.Id)
            .Skip(skip).Take(take)
            .ToListAsync();
        var totalCount = await _context.Notifications
            .Where(x => x.UserId == user.Id)
            .CountAsync();

        return new ResponsePaging<IList<Notification>>
        {
            StatusCode = HttpStatusCode.OK,
            TotalCount = totalCount,
            ResultCount = notifications.Count,
            Message = $"Got {notifications.Count} notifications",
            Data = notifications
        };
    }

    #endregion

    #region Update

    /// <inheritdoc />
    public async Task ReadNotification(int notificationId, HttpRequest request)
    {
        var notification = await _context.Notifications.FindAsync(notificationId) ??
            throw new NotificationNotFoundException($"Notification '{notificationId}' not found");
        notification.Read = true;
        await _context.SaveChangesAsync();
    }

    #endregion
}
