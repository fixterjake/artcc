using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ZDC.Server.Data;
using ZDC.Server.Repositories.Interfaces;
using ZDC.Shared.Dtos;
using ZDC.Shared.Models;

namespace ZDC.Server.Repositories;

public class StatsRepository : IStatsRepository
{
    private readonly DatabaseContext _context;
    private readonly IMapper _mapper;

    public StatsRepository(DatabaseContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<IList<StatsDto>> GetStats(int month, int year)
    {
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
        return results;
    }
}
