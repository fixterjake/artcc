using ZDC.Shared.Dtos;
using ZDC.Shared.Models;

namespace ZDC.Server.Repositories.Interfaces;

public interface IControllerLogRepository
{
    Task<Response<IList<ControllerLog>>> GetUserControllerLogs(int id);
}