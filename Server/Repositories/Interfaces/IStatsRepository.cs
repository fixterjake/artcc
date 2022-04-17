using ZDC.Shared.Dtos;

namespace ZDC.Server.Repositories.Interfaces;

public interface IStatsRepository
{
    Task<Response<IList<StatsDto>>> GetStats(int month, int year);
}
