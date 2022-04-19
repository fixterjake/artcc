using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using System.Net;
using ZDC.Server.Data;
using ZDC.Server.Repositories.Interfaces;
using ZDC.Shared.Dtos;
using ZDC.Shared.Models;

namespace ZDC.Server.Repositories;

public class StatsRepository : IStatsRepository
{
    private readonly DatabaseContext _context;
    private readonly IDistributedCache _cache;
    private readonly IMapper _mapper;

    public StatsRepository(DatabaseContext context, IDistributedCache cache, IMapper mapper)
    {
        _context = context;
        _cache = cache;
        _mapper = mapper;
    }

    /// <inheritdoc />
    public async Task<Response<IList<StatsDto>>> GetStats(int month, int year)
    {
        if (month == 0 || year == 0)
        {
            month = DateTimeOffset.UtcNow.Month;
            year = DateTimeOffset.UtcNow.Year;
        }

        var cachedStats = await _cache.GetStringAsync($"_stats_{month}_{year}");
        if (!string.IsNullOrEmpty(cachedStats))
        {
            var stats = JsonConvert.DeserializeObject<List<StatsDto>>(cachedStats);
            if (stats != null)
                return new Response<IList<StatsDto>>
                {
                    StatusCode = HttpStatusCode.OK,
                    Message = $"Got {stats.Count} stats",
                    Data = stats
                };
        }

        var results = new List<StatsDto>();
        var users = await _context.Users
            .Where(x => x.Status != UserStatus.Removed)
            .OrderBy(x => x.LastName)
            .ToListAsync();
        foreach (var entry in users)
        {
            var hours = await _context.Hours
                .Where(x => x.UserId == entry.Id)
                .FirstOrDefaultAsync(x => x.Month == month && x.Year == year) ??
                new Hours
                {
                    UserId = entry.Id,
                    Month = month,
                    Year = year,
                    LocalHours = TimeSpan.Zero,
                    TraconHours = TimeSpan.Zero,
                    CenterHours = TimeSpan.Zero
                };
            results.Add(new StatsDto
            {
                FullName = entry.FullName,
                RatingLong = entry.RatingLong,
                Status = entry.Status,
                Visitor = entry.Visitor,
                Hours = _mapper.Map<Hours, HoursDto>(hours)
            });
        }
        var expiryOptions = new DistributedCacheEntryOptions()
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(2),
            SlidingExpiration = TimeSpan.FromMinutes(1)
        };
        await _cache.SetStringAsync($"_stats_{month}_{year}", JsonConvert.SerializeObject(results), expiryOptions);

        return new Response<IList<StatsDto>>
        {
            StatusCode = HttpStatusCode.OK,
            Message = $"Got {results.Count} stats",
            Data = results
        };
    }
}
