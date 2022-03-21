using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
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

public class EventRepository : IEventRepository
{
    private readonly DatabaseContext _context;
    private readonly ILoggingService _loggingService;
    private readonly IEmailService _emailService;

    public EventRepository(DatabaseContext context, ILoggingService loggingService, IEmailService emailService)
    {
        _context = context;
        _loggingService = loggingService;
        _emailService = emailService;
    }

    public async Task<Response<Event>> CreateEvent(Event @event, HttpRequest request)
    {
        var result = await _context.Events.AddAsync(@event);
        await _context.SaveChangesAsync();
        var newData = JsonConvert.SerializeObject(result.Entity);

        await _loggingService.AddWebsiteLog(request, $"Created event '{result.Entity.Id}'", string.Empty, newData);

        return new Response<Event>
        {
            StatusCode = HttpStatusCode.Created,
            Message = $"Created event '{result.Entity.Id}'",
            Data = result.Entity
        };
    }

    public async Task<Response<EventPosition>> CreateEventPosition(EventPosition position, HttpRequest request)
    {
        var result = await _context.EventsPositions.AddAsync(position);
        await _context.SaveChangesAsync();
        var newData = JsonConvert.SerializeObject(result.Entity);

        await _loggingService.AddWebsiteLog(request, $"Created event position '{result.Entity.Id}'", string.Empty, newData);

        return new Response<EventPosition>
        {
            StatusCode = HttpStatusCode.Created,
            Message = $"Created event position '{result.Entity.Id}'",
            Data = result.Entity
        };
    }

    public async Task<Response<EventRegistration>> CreateEventRegistration(EventRegistration registration, HttpRequest request)
    {
        var user = await _context.Users.FindAsync(registration.UserId);
        if (user == null)
            throw new UserNotFoundException($"User '{registration.UserId}' not found");

        var @event = await _context.Events.FindAsync(registration.UserId);
        if (@event == null)
            throw new EventNotFoundException($"Event '{registration.EventId}' not found");

        var result = await _context.EventsRegistrations.AddAsync(registration);
        await _context.SaveChangesAsync();
        var newData = JsonConvert.SerializeObject(result.Entity);

        await _loggingService.AddWebsiteLog(request, $"Created event registration '{result.Entity.Id}'", string.Empty, newData);
        await _emailService.SendEventRegistrationSubmitted(user.Email, @event.Id, @event.Name, registration.Start, registration.End);

        return new Response<EventRegistration>
        {
            StatusCode = HttpStatusCode.Created,
            Message = $"Created event registration '{result.Entity.Id}'",
            Data = result.Entity
        };
    }

    public async Task<Response<IList<Event>>> GetEvents()
    {
        var events = await _context.Events.ToListAsync();
        return new Response<IList<Event>>
        {
            StatusCode = HttpStatusCode.OK,
            Message = $"Got {events.Count} events",
            Data = events
        };
    }

    public async Task<Response<Event>> GetEvent(int id)
    {
        var @event = await _context.Events
            .Include(x => x.Upload)
            .Include(x => x.Positions)
            .FirstOrDefaultAsync(x => x.Id == id);
        if (@event == null)
            throw new EventNotFoundException($"Event '{id}' not found");

        return new Response<Event>
        {
            StatusCode = HttpStatusCode.OK,
            Message = $"Got event '{id}'",
            Data = @event
        };
    }

    public async Task<Response<IList<EventPosition>>> GetEventPositions(int id)
    {
        var @event = await _context.Events
            .Include(x => x.Upload)
            .Include(x => x.Positions)
            .FirstOrDefaultAsync(x => x.Id == id);
        if (@event == null)
            throw new EventNotFoundException($"Event '{id}' not found");

        return new Response<IList<EventPosition>>
        {
            StatusCode = HttpStatusCode.OK,
            Message = $"Got {@event.Positions?.Count} event positions",
            Data = @event.Positions
        };
    }

    public async Task<Response<IList<EventRegistration>>> GetEventRegistrations(int id)
    {
        var @event = await _context.Events
            .Include(x => x.Upload)
            .Include(x => x.Positions)
            .FirstOrDefaultAsync(x => x.Id == id);
        if (@event == null)
            throw new EventNotFoundException($"Event '{id}' not found");

        var registrations = await _context.EventsRegistrations
            .Where(x => x.EventId == @event.Id)
            .ToListAsync();

        return new Response<IList<EventRegistration>>
        {
            StatusCode = HttpStatusCode.OK,
            Message = $"Got {registrations.Count} event registrations",
            Data = registrations
        };
    }

    public async Task<Response<EventRegistration>> GetUserEventRegistration(int eventId, HttpRequest request)
    {
        var user = request.HttpContext.GetUser(_context);
        if (user == null)
            throw new UserNotFoundException($"User '{request.HttpContext.GetCid}' not found");

        var @event = await _context.Events.FindAsync(eventId);
        if (@event == null)
            throw new EventNotFoundException($"Event '{eventId}' not found");

        var registration = await _context.EventsRegistrations
            .Where(x => x.EventId == @event.Id)
            .FirstOrDefaultAsync(x => x.UserId == user.Id);
        if (registration == null)
            throw new EventRegistrationNotFoundException($"Event registration not found");

        return new Response<EventRegistration>
        {
            StatusCode = HttpStatusCode.OK,
            Message = $"Got event registration '{registration.Id}'",
            Data = registration
        };
    }

    public async Task<Response<Event>> UpdateEvent(Event @event, HttpRequest request)
    {
        var dbEvent = await _context.Events.AsNoTracking().FirstOrDefaultAsync(x => x.Id == @event.Id);
        if (dbEvent == null)
            throw new EventNotFoundException($"Event '{@event.Id}' not found");

        var oldData = JsonConvert.SerializeObject(dbEvent);
        var result = _context.Events.Update(@event);
        await _context.SaveChangesAsync();
        var newData = JsonConvert.SerializeObject(result.Entity);

        await _loggingService.AddWebsiteLog(request, $"Updated event '{result.Entity.Id}'", oldData, newData);

        return new Response<Event>
        {
            StatusCode = HttpStatusCode.OK,
            Message = $"Updated event '{@event.Id}'",
            Data = result.Entity
        };
    }

    public async Task<Response<EventPosition>> UpdateEventPosition(EventPosition position, HttpRequest request)
    {
        var dbPosition = await _context.EventsPositions.AsNoTracking().FirstOrDefaultAsync(x => x.Id == position.Id);
        if (dbPosition == null)
            throw new EventPositionNotFoundException($"Event position '{position.Id}' not found");

        var oldData = JsonConvert.SerializeObject(dbPosition);
        var result = _context.EventsPositions.Update(position);
        await _context.SaveChangesAsync();
        var newData = JsonConvert.SerializeObject(result.Entity);

        await _loggingService.AddWebsiteLog(request, $"Updated event position '{result.Entity.Id}'", oldData, newData);

        return new Response<EventPosition>
        {
            StatusCode = HttpStatusCode.OK,
            Message = $"Updated event position '{position.Id}'",
            Data = result.Entity
        };
    }

    public async Task<Response<EventRegistration>> UpdateEventRegistration(EventRegistration registration, HttpRequest request)
    {
        var dbRegistration = await _context.EventsRegistrations.AsNoTracking().FirstOrDefaultAsync(x => x.Id == registration.Id);
        if (dbRegistration == null)
            throw new EventRegistrationNotFoundException($"Event registration '{registration.Id}' not found");

        var oldData = JsonConvert.SerializeObject(dbRegistration);
        var result = _context.EventsRegistrations.Update(registration);
        await _context.SaveChangesAsync();
        var newData = JsonConvert.SerializeObject(result.Entity);

        await _loggingService.AddWebsiteLog(request, $"Updated event registration '{result.Entity.Id}'", oldData, newData);

        return new Response<EventRegistration>
        {
            StatusCode = HttpStatusCode.OK,
            Message = $"Updated event registration '{registration.Id}'",
            Data = result.Entity
        };
    }

    public async Task<Response<IList<EventRegistration>>> AssignReliefPositions(int eventId, HttpRequest request)
    {
        var @event = await _context.Events
            .Include(x => x.Positions)
            .FirstOrDefaultAsync(x => x.Id == eventId);
        if (@event == null)
            throw new EventNotFoundException($"Event '{eventId}' not found");

        var registrations = await _context.EventsRegistrations
            .Include(x => x.Position)
            .Where(x => x.Id == eventId)
            .Where(x => x.Status == EventRegistrationStatus.Pending)
            .ToListAsync();

        foreach (var entry in registrations)
        {
            var user = await _context.Users.FindAsync(entry.UserId);
            if (user == null)
                continue;

            var oldData = JsonConvert.SerializeObject(entry);
            entry.Status = EventRegistrationStatus.Relief;
            await _context.SaveChangesAsync();
            var newData = JsonConvert.SerializeObject(entry);

            await _loggingService.AddWebsiteLog(request, $"Updated event registration '{entry.Id}'", oldData, newData);
            await _emailService.SendEventPositionAssigned(user.Email, eventId, @event.Name, "Relief", entry.Start, entry.End);
        }

        return new Response<IList<EventRegistration>>
        {
            StatusCode = HttpStatusCode.OK,
            Message = $"Assigned {registrations.Count} as relief for event {eventId}",
            Data = registrations
        };
    }

    public Task<Response<EventRegistration>> AssignEventPositions(int registrationId, int positionId, HttpRequest request)
    {
        throw new NotImplementedException();
    }

    public Task<Response<EventRegistration>> UnAssignEventPositions(int registrationId, int positionId, HttpRequest request)
    {
        throw new NotImplementedException();
    }

    public Task<Response<Event>> DeleteEvent(int id, HttpRequest request)
    {
        throw new NotImplementedException();
    }

    public Task<Response<EventPosition>> DeleteEventPosition(int id, HttpRequest request)
    {
        throw new NotImplementedException();
    }

    public Task<Response<EventRegistration>> DeleteEventRegistration(int id, HttpRequest request)
    {
        throw new NotImplementedException();
    }
}
