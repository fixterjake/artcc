using ZDC.Shared.Dtos;
using ZDC.Shared.Models;

namespace ZDC.Server.Repositories.Interfaces;

public interface IDebugLogRepository
{
    Task<Response<IList<DebugLog>>> GetDebugLogs();
}