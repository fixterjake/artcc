using ZDC.Shared.Dtos;

namespace ZDC.Server.Repositories.Interfaces;

public interface IStatsRepository
{
    Task<IList<StatsDto>> GetStats(int month, int year);
}
