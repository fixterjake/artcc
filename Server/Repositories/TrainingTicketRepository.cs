using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using System.Net;
using ZDC.Server.Data;
using ZDC.Server.Extensions;
using ZDC.Server.Repositories.Interfaces;
using ZDC.Server.Services.Interfaces;
using ZDC.Shared;
using ZDC.Shared.Dtos;
using ZDC.Shared.Models;

namespace ZDC.Server.Repositories;

public class TrainingTicketRepository : ITrainingTicketRepository
{
    private readonly DatabaseContext _context;
    private readonly IDistributedCache _cache;
    private readonly IVatusaService _vatusaService;
    private readonly IMapper _mapper;
    private readonly ILoggingService _loggingService;

    public TrainingTicketRepository(DatabaseContext context, IDistributedCache cache,
        IVatusaService vatusaService, IMapper mapper, ILoggingService loggingService)
    {
        _context = context;
        _cache = cache;
        _vatusaService = vatusaService;
        _mapper = mapper;
        _loggingService = loggingService;
    }

    #region Create

    /// <inheritdoc />
    public async Task<Response<TrainingTicket>> CreateTrainingTicket(TrainingTicket trainingTicket, HttpRequest request)
    {
        if (!await _context.Users.AnyAsync(x => x.Id == trainingTicket.UserId))
            throw new UserNotFoundException($"User '{trainingTicket.UserId}' not found");
        if (!await _context.Users.AnyAsync(x => x.Id == trainingTicket.TrainerId))
            throw new UserNotFoundException($"User '{trainingTicket.TrainerId}' not found");

        var result = await _context.TrainingTickets.AddAsync(trainingTicket);
        await _context.SaveChangesAsync();
        var newData = JsonConvert.SerializeObject(result.Entity);

        await _loggingService.AddWebsiteLog(request, $"Created training ticket '{result.Entity.Id}'", string.Empty, newData);
        await _vatusaService.AddTrainingTicket(result.Entity);

        return new Response<TrainingTicket>
        {
            StatusCode = HttpStatusCode.OK,
            Message = $"Created training ticket '{result.Entity.Id}'",
            Data = result.Entity
        };
    }

    #endregion

    #region Read

    /// <inheritdoc />
    public async Task<ResponsePaging<IList<TrainingTicket>>> GetTrainingTickets(int skip, int take)
    {
        var trainingTickets = await _context.TrainingTickets
            .Skip(skip).Take(take)
            .ToListAsync();
        var totalCount = await _context.TrainingTickets.CountAsync();

        return new ResponsePaging<IList<TrainingTicket>>
        {
            StatusCode = HttpStatusCode.OK,
            TotalCount = totalCount,
            ResultCount = trainingTickets.Count,
            Message = $"Got {trainingTickets.Count} training tickets",
            Data = trainingTickets
        };
    }

    /// <inheritdoc />
    public async Task<ResponsePaging<IList<TrainingTicket>>> GetUserTrainingTickets(int userId, int skip, int take)
    {
        if (!await _context.Users.AnyAsync(x => x.Id == userId))
            throw new UserNotFoundException($"User '{userId}' not found");

        var trainingTicket = await _context.TrainingTickets
            .Where(x => x.UserId == userId)
            .Skip(skip).Take(take)
            .ToListAsync();
        var totalCount = await _context.TrainingTickets
            .Where(x => x.UserId == userId)
            .CountAsync();

        return new ResponsePaging<IList<TrainingTicket>>
        {
            StatusCode = HttpStatusCode.OK,
            TotalCount = totalCount,
            ResultCount = trainingTicket.Count,
            Message = $"Got {trainingTicket.Count} training tickets",
            Data = trainingTicket
        };
    }

    /// <inheritdoc />
    public async Task<ResponsePaging<IList<TrainingTicketDto>>> GetUserTrainingTickets(int skip, int take, HttpRequest request)
    {
        var user = await request.HttpContext.GetUser(_context);
        if (user == null)
            throw new UserNotFoundException("User not found");

        var cachedTickets = await _cache.GetStringAsync($"_trainingtickets_{user.Id}");
        var cachedCount = await _cache.GetStringAsync($"_trainingtickets_{user.Id}_count");
        if (!string.IsNullOrEmpty(cachedTickets))
        {
            var trainingTickets = JsonConvert.DeserializeObject<List<TrainingTicketDto>>(cachedTickets);
            var count = int.Parse(cachedCount);
            if (trainingTickets != null)
                return new ResponsePaging<IList<TrainingTicketDto>>
                {
                    StatusCode = HttpStatusCode.OK,
                    TotalCount = count,
                    ResultCount = trainingTickets.Count,
                    Message = $"Got {trainingTickets.Count} training tickets",
                    Data = trainingTickets
                };
        }

        var dbTrainingTickets = await _context.TrainingTickets
            .Where(x => x.UserId == user.Id)
            .Skip(skip).Take(take)
            .ToListAsync();
        var totalCount = await _context.TrainingTickets
            .Where(x => x.UserId == user.Id)
            .CountAsync();

        var expiryOptions = new DistributedCacheEntryOptions()
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(2),
            SlidingExpiration = TimeSpan.FromMinutes(1)
        };
        await _cache.SetStringAsync($"_trainingtickets_{user.Id}", JsonConvert.SerializeObject(dbTrainingTickets), expiryOptions);
        await _cache.SetStringAsync($"_trainingtickets_{user.Id}_count", $"{totalCount}", expiryOptions);

        return new ResponsePaging<IList<TrainingTicketDto>>
        {
            StatusCode = HttpStatusCode.OK,
            TotalCount = totalCount,
            ResultCount = dbTrainingTickets.Count,
            Message = $"Got {dbTrainingTickets.Count} training tickets",
            Data = _mapper.Map<IList<TrainingTicket>, IList<TrainingTicketDto>>(dbTrainingTickets)
        };
    }

    /// <inheritdoc />
    public async Task<Response<TrainingTicket>> GetTrainingTicket(int trainingTicketId)
    {
        var trainingTicket = await _context.TrainingTickets
            .Include(x => x.User)
            .Include(x => x.Trainer)
            .FirstOrDefaultAsync(x => x.Id != trainingTicketId) ??
            throw new TrainingTicketNotFoundException($"Training ticket '{trainingTicketId}' not found");

        return new Response<TrainingTicket>
        {
            StatusCode = HttpStatusCode.OK,
            Message = $"Got training ticket '{trainingTicketId}'",
            Data = trainingTicket
        };
    }

    #endregion

    #region Update

    /// <inheritdoc />
    public async Task<Response<TrainingTicket>> UpdateTrainingTicket(TrainingTicket trainingTicket, HttpRequest request)
    {
        var dbTrainingTicket = await _context.TrainingTickets.AsNoTracking().FirstOrDefaultAsync(x => x.Id == trainingTicket.Id) ??
            throw new TrainingTicketNotFoundException($"Training ticket '{trainingTicket.Id}' not found");

        var oldData = JsonConvert.SerializeObject(dbTrainingTicket);
        trainingTicket.Updated = DateTimeOffset.UtcNow;
        var result = _context.TrainingTickets.Update(trainingTicket);
        await _context.SaveChangesAsync();
        var newData = JsonConvert.SerializeObject(result.Entity);

        await _loggingService.AddWebsiteLog(request, $"Updated training ticket '{trainingTicket.Id}'", oldData, newData);

        return new Response<TrainingTicket>
        {
            StatusCode = HttpStatusCode.OK,
            Message = $"Updated training ticket '{trainingTicket.Id}'",
            Data = result.Entity
        };
    }

    #endregion

    #region Delete

    /// <inheritdoc />
    public async Task<Response<TrainingTicket>> DeleteTrainingTicket(int trainingTicketId, HttpRequest request)
    {
        var trainingTicket = await _context.TrainingTickets.FindAsync(trainingTicketId) ??
            throw new TrainingTicketNotFoundException($"Training ticket '{trainingTicketId}' not found");

        var oldData = JsonConvert.SerializeObject(trainingTicket);
        var result = _context.TrainingTickets.Remove(trainingTicket);
        await _context.SaveChangesAsync();

        await _loggingService.AddWebsiteLog(request, $"Deleted training ticket '{trainingTicketId}'", oldData, string.Empty);

        return new Response<TrainingTicket>
        {
            StatusCode = HttpStatusCode.OK,
            Message = $"Deleted training ticket '{trainingTicketId}'",
            Data = result.Entity
        };
    }

    #endregion
}
