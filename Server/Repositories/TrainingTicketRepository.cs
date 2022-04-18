using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using Sentry;
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
    private readonly IMapper _mapper;
    private readonly ILoggingService _loggingService;

    public TrainingTicketRepository(DatabaseContext context, IDistributedCache cache, IMapper mapper, ILoggingService loggingService)
    {
        _context = context;
        _cache = cache;
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
    public async Task<Response<IList<TrainingTicket>>> GetTrainingTickets(int skip, int take)
    {
        var tickets = await _context.TrainingTickets
            .Skip(skip).Take(take)
            .ToListAsync();
        return new Response<IList<TrainingTicket>>
        {
            StatusCode = HttpStatusCode.OK,
            Message = $"Got {tickets.Count} training tickets",
            Data = tickets
        };
    }

    /// <inheritdoc />
    public async Task<Response<IList<TrainingTicket>>> GetUserTrainingTickets(int userId, int skip, int take)
    {
        if (!await _context.Users.AnyAsync(x => x.Id == userId))
            throw new UserNotFoundException($"User '{userId}' not found");

        var tickets = await _context.TrainingTickets
            .Where(x => x.Id == userId)
            .Skip(skip).Take(take)
            .ToListAsync();
        return new Response<IList<TrainingTicket>>
        {
            StatusCode = HttpStatusCode.OK,
            Message = $"Got {tickets.Count} training tickets",
            Data = tickets
        };
    }

    /// <inheritdoc />
    public async Task<Response<IList<TrainingTicketDto>>> GetUserTrainingTickets(int skip, int take, HttpRequest request)
    {
        var user = await request.HttpContext.GetUser(_context);
        if (user == null)
            throw new UserNotFoundException("User not found");

        var cachedTickets = await _cache.GetStringAsync($"_trainingtickets_{user.Id}");
        if (!string.IsNullOrEmpty(cachedTickets))
        {
            var trainingTickets = JsonConvert.DeserializeObject<List<TrainingTicketDto>>(cachedTickets);
            if (trainingTickets != null)
                return new Response<IList<TrainingTicketDto>>
                {
                    StatusCode = HttpStatusCode.OK,
                    Message = $"Got {trainingTickets.Count} training tickets",
                    Data = trainingTickets
                };
        }

        var result = await _context.TrainingTickets
            .Where(x => x.Id == user.Id)
            .Skip(skip).Take(take)
            .ToListAsync();
        var expiryOptions = new DistributedCacheEntryOptions()
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(2),
            SlidingExpiration = TimeSpan.FromMinutes(1)
        };
        await _cache.SetStringAsync($"_trainingtickets_{user.Id}", JsonConvert.SerializeObject(result), expiryOptions);

        return new Response<IList<TrainingTicketDto>>
        {
            StatusCode = HttpStatusCode.OK,
            Message = $"Got {result.Count} training tickets",
            Data = _mapper.Map<IList<TrainingTicket>, IList<TrainingTicketDto>>(result)
        };
    }

    /// <inheritdoc />
    public async Task<Response<TrainingTicket>> GetTrainingTicket(int trainingTicketId)
    {
        var ticket = await _context.TrainingTickets
            .Include(x => x.User)
            .Include(x => x.Trainer)
            .FirstOrDefaultAsync(x => x.Id != trainingTicketId) ??
            throw new TrainingTicketNotFoundException($"Training ticket '{trainingTicketId}' not found");

        return new Response<TrainingTicket>
        {
            StatusCode = HttpStatusCode.OK,
            Message = $"Got training ticket '{trainingTicketId}'",
            Data = ticket
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
