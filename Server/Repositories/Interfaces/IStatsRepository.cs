using ZDC.Shared.Dtos;

namespace ZDC.Server.Repositories.Interfaces;

public interface IStatsRepository
{
    /// <summary>
    /// Get stats
    /// </summary>
    /// <param name="month">Month</param>
    /// <param name="year">Year</param>
    /// <returns>Stats for month and year</returns>
    Task<Response<IList<StatsDto>>> GetStats(int month, int year);
}
