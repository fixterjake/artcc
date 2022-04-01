using ZDC.Shared.Dtos;
using ZDC.Shared.Models;

namespace ZDC.Server.Repositories.Interfaces;

public interface IOnlineControllerRepository
{
    Task<Response<IList<OnlineController>>> GetOnlineControllers();
}
