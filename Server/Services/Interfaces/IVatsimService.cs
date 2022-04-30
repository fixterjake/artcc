using ZDC.Shared.Models;

namespace ZDC.Server.Services.Interfaces;

public interface IVatsimService
{
    Task<double> GetUserRatingHours(int userId, Rating rating);
    Task<DateTime> GetLastRatingChange(int userId);
}
