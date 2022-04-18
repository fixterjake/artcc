using ZDC.Shared.Dtos;
using ZDC.Shared.Models;

namespace ZDC.Server.Repositories.Interfaces;

public interface INotificationRepository
{
    /// <summary>
    /// Add senior staff notification
    /// </summary>
    /// <param name="title">Title</param>
    /// <param name="description">Description</param>
    /// <param name="link">Link</param>
    Task AddSeniorStaffNotification(string title, string description, string link);

    /// <summary>
    /// Add staff notification
    /// </summary>
    /// <param name="title">Title</param>
    /// <param name="description">Description</param>
    /// <param name="link">Link</param>
    Task AddStaffNotification(string title, string description, string link);

    /// <summary>
    /// Add training staff notification
    /// </summary>
    /// <param name="title">Title</param>
    /// <param name="description">Description</param>
    /// <param name="link">Link</param>
    Task AddTrainingNotification(string title, string description, string link);

    /// <summary>
    /// Add events staff notification
    /// </summary>
    /// <param name="title">Title</param>
    /// <param name="description">Description</param>
    /// <param name="link">Link</param>
    Task AddEventsNotification(string title, string description, string link);

    /// <summary>
    /// Add all staff notification
    /// </summary>
    /// <param name="title">Title</param>
    /// <param name="description">Description</param>
    /// <param name="link">Link</param>
    Task AddAllNotification(string title, string description, string link);

    /// <summary>
    /// Get notifications
    /// </summary>
    /// <param name="skip">Number to skip</param>
    /// <param name="take">Number to take</param>
    /// <param name="request">Raw http request</param>
    /// <exception cref="Shared.UserNotFoundException">User not found</exception>
    /// <returns>Notifications</returns>
    Task<ResponsePaging<IList<Notification>>> GetNotifications(int skip, int take, HttpRequest request);

    /// <summary>
    /// Set a notification as read
    /// </summary>
    /// <param name="notificationId">Notification id</param>
    /// <param name="request">Raw http request</param>
    /// <exception cref="Shared.NotificationNotFoundException">Notification not found</exception>
    Task ReadNotification(int notificationId, HttpRequest request);
}
