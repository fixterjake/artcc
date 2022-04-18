using ZDC.Shared.Dtos;
using ZDC.Shared.Models;

namespace ZDC.Server.Repositories.Interfaces;

public interface IAnnouncementRepository
{
    /// <summary>
    /// Create announcement
    /// </summary>
    /// <param name="announcement">Announcement to create</param>
    /// <param name="request">Raw http request</param>
    /// <exception cref="Shared.UserNotFoundException">Posting user not found</exception>
    /// <returns>Created announcement</returns>
    Task<Response<Announcement>> CreateAnnouncement(Announcement announcement, HttpRequest request);

    /// <summary>
    /// Get all announcements
    /// </summary>
    /// <returns>All announcements</returns>
    Task<Response<IList<Announcement>>> GetAnnouncements();

    /// <summary>
    /// Get announcement by id
    /// </summary>
    /// <param name="announcementId">Announcement id to get</param>
    /// <exception cref="Shared.AnnouncementNotFoundException">Announcement not found</exception>
    /// <returns>Announcement if found</returns>
    Task<Response<Announcement>> GetAnnouncement(int announcementId);

    /// <summary>
    /// Update an announcement
    /// </summary>
    /// <param name="announcement">Updated announcement</param>
    /// <param name="request">Raw http request</param>
    /// <exception cref="Shared.AnnouncementNotFoundException">Announcement not found</exception>
    /// <exception cref="Shared.UserNotFoundException">Posting user not found</exception>
    /// <returns>Updated announcement</returns>
    Task<Response<Announcement>> UpdateAnnouncement(Announcement announcement, HttpRequest request);

    /// <summary>
    /// Delete an announcement
    /// </summary>
    /// <param name="announcementId">Announcement id</param>
    /// <param name="request">Raw http request</param>
    /// <exception cref="Shared.AnnouncementNotFoundException">Announcement not found</exception>
    /// <returns>Deleted announcement</returns>
    Task<Response<Announcement>> DeleteAnnouncement(int announcementId, HttpRequest request);
}
