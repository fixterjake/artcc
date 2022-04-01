using ZDC.Shared.Dtos;
using ZDC.Shared.Models;

namespace ZDC.Server.Repositories.Interfaces;

public interface INotificationRepository
{
    Task AddSeniorStaffNotification(string title, string description, string link);
    Task AddStaffNotification(string title, string description, string link);
    Task AddTrainingNotification(string title, string description, string link);
    Task AddAllNotification(string title, string description, string link);
    Task<Response<IList<Notification>>> GetNotifications(HttpRequest request);
    Task ReadNotification(int notificationId, HttpRequest request);
}
