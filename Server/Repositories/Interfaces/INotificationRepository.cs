using ZDC.Shared.Dtos;
using ZDC.Shared.Models;

namespace ZDC.Server.Repositories.Interfaces;

public interface INotificationRepository
{
    Task AddNotification(Notification notification);
    Task<Response<IList<Notification>>> GetNotifications(HttpRequest request);
    Task ReadNotification(int notificationId, HttpRequest request);
}
