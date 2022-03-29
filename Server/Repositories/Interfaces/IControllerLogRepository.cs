using ZDC.Shared.Dtos;

namespace ZDC.Server.Repositories.Interfaces;

public interface IControllerLogRepository
{
    Task<Response<IList<ControllerLogDto>>> GetUserControllerLogs(int id);
}