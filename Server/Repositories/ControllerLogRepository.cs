using System.Net;
using Amazon.Runtime.Internal.Util;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using ZDC.Server.Data;
using ZDC.Server.Repositories.Interfaces;
using ZDC.Shared;
using ZDC.Shared.Dtos;
using ZDC.Shared.Models;

namespace ZDC.Server.Repositories;

public class ControllerLogRepository : IControllerLogRepository
{
    private readonly DatabaseContext _context;
    private readonly IDistributedCache _cache;
    private readonly IMapper _mapper;

    public ControllerLogRepository(DatabaseContext context, IDistributedCache cache, IMapper mapper)
    {
        _context = context;
        _cache = cache;
        _mapper = mapper;
    }

    /// <inheritdoc />
    public async Task<ResponsePaging<IList<ControllerLogDto>>> GetUserControllerLogs(int userId, int skip, int take)
    {
        if (!_context.Users.Any(x => x.Id == userId))
            throw new UserNotFoundException($"User '{userId}' not found");

        var cachedControllerLogs = await _cache.GetStringAsync($"_controllerlogs_{userId}_{skip}_{take}");
        var cachedCount = await _cache.GetStringAsync($"_controllerlogs_{userId}_count");
        if (!string.IsNullOrEmpty(cachedControllerLogs))
        {
            var controllerLogs = JsonConvert.DeserializeObject<IList<ControllerLog>>(cachedControllerLogs);
            var count = int.Parse(cachedCount);
            if (controllerLogs != null)
                return new ResponsePaging<IList<ControllerLogDto>>
                {
                    StatusCode = HttpStatusCode.OK,
                    TotalCount = count,
                    ResultCount = controllerLogs.Count,
                    Message = $"Got {controllerLogs.Count} controller logs for user '{userId}'",
                    Data = _mapper.Map<IList<ControllerLog>, IList<ControllerLogDto>>(controllerLogs)
                };
        }

        var result = await _context.ControllerLogs
            .Where(x => x.UserId == userId)
            .Skip(skip).Take(take)
            .ToListAsync();
        var totalCount = await _context.ControllerLogs.CountAsync();

        var expiryOptions = new DistributedCacheEntryOptions()
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(2),
            SlidingExpiration = TimeSpan.FromMinutes(1)
        };
        await _cache.SetStringAsync($"_controllerlogs_{userId}_{skip}_{take}", JsonConvert.SerializeObject(result), expiryOptions);
        await _cache.SetStringAsync($"_controllerlogs_{userId}_count", $"{totalCount}", expiryOptions);

        return new ResponsePaging<IList<ControllerLogDto>>
        {
            StatusCode = HttpStatusCode.OK,
            TotalCount = totalCount,
            ResultCount = result.Count,
            Message = $"Got {result.Count} controller logs for user '{userId}'",
            Data = _mapper.Map<IList<ControllerLog>, IList<ControllerLogDto>>(result)
        };
    }
}