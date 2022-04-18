using ZDC.Shared.Dtos;
using ZDC.Shared.Models;

namespace ZDC.Server.Repositories.Interfaces;

public interface IOnlineControllerRepository
{
    /// <summary>
    /// Get online controllers
    /// </summary>
    /// <returns>Online controllers</returns>
    Task<Response<IList<OnlineController>>> GetOnlineControllers();
}
