using ZDC.Shared.Dtos;
using ZDC.Shared.Models;

namespace ZDC.Server.Repositories.Interfaces;

public interface IAnnouncementRepository
{
    Task<Response<Announcement>> CreateAnnouncement(Announcement announcement, HttpRequest request);
    Task<Response<IList<Announcement>>> GetAnnouncements();
    Task<Response<Announcement>> GetAnnouncement(int announcementId);
    Task<Response<Announcement>> UpdateAnnouncement(Announcement announcement, HttpRequest request);
    Task<Response<Announcement>> DeleteAnnouncement(int announcementId, HttpRequest request);
}
