using ZDC.Shared.Dtos;

namespace ZDC.Server.Repositories.Interfaces;

public interface IControllerLogRepository
{
    /// <summary>
    /// Get a users controller logs
    /// </summary>
    /// <param name="userId">User id</param>
    /// <exception cref="Shared.UserNotFoundException">User not found</exception>
    /// <returns>Users controller logs</returns>
    Task<Response<IList<ControllerLogDto>>> GetUserControllerLogs(int userId);
}