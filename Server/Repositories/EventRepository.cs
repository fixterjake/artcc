using Amazon.Runtime.Internal;
using Microsoft.EntityFrameworkCore;
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

    #region Create

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

    public async Task<Response<EventPosition>> CreateEventPosition(EventPosition position, int eventId, HttpRequest request)
    {
        var @event = await _context.Events
            .Include(x => x.Positions)
            .FirstOrDefaultAsync(x => x.Id == eventId);
        if (@event == null)
            throw new EventNotFoundException($"Event '{eventId}' not found");

        var oldData = JsonConvert.SerializeObject(@event);
        @event.Positions?.Add(position);
        await _context.SaveChangesAsync();
        var newData = JsonConvert.SerializeObject(@event);

        await _loggingService.AddWebsiteLog(request, $"Added event position to event '{eventId}'", oldData, newData);

        return new Response<EventPosition>
        {
            StatusCode = HttpStatusCode.Created,
            Message = $"Added event position to event '{eventId}'",
            Data = position
        };
    }

    public async Task<Response<EventRegistration>> CreateEventRegistration(EventRegistration registration, HttpRequest request)
    {
        var user = await request.HttpContext.GetUser(_context);
        if (user == null)
            throw new UserNotFoundException($"User not found");

        var @event = await _context.Events.FindAsync(registration.EventId);
        if (@event == null)
            throw new EventNotFoundException($"Event '{registration.EventId}' not found");

        registration.UserId = user.Id;

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

    #endregion

    #region Read

    public async Task<Response<IList<Event>>> GetEvents(HttpRequest request)
    {
        var events = await _context.Events
            .Include(x => x.Upload)
            .Include(x => x.Positions)
            .ToListAsync();
        if (!await request.HttpContext.IsStaff(_context))
            events = events.Where(x => x.Open).ToList();
        return new Response<IList<Event>>
        {
            StatusCode = HttpStatusCode.OK,
            Message = $"Got {events.Count} events",
            Data = events
        };
    }

    public async Task<Response<Event>> GetEvent(int eventId, HttpRequest request)
    {
        var @event = await _context.Events
            .Include(x => x.Upload)
            .Include(x => x.Positions)
            .FirstOrDefaultAsync(x => x.Id == eventId) ??
            throw new EventNotFoundException($"Event '{eventId}' not found");
        if (!await request.HttpContext.IsStaff(_context) && !@event.Open)
            throw new EventNotFoundException($"Event '{eventId}' not found");

        return new Response<Event>
        {
            StatusCode = HttpStatusCode.OK,
            Message = $"Got event '{eventId}'",
            Data = @event
        };
    }

    public async Task<Response<IList<EventRegistration>>> GetEventRegistrations(int eventId)
    {
        var @event = await _context.Events
            .Include(x => x.Upload)
            .Include(x => x.Positions)
            .FirstOrDefaultAsync(x => x.Id == eventId) ??
            throw new EventNotFoundException($"Event '{eventId}' not found");

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
        var registration = await request.HttpContext.GetUserEventRegistration(_context, eventId) ??
            throw new EventRegistrationNotFoundException($"Event registration not found");
        return new Response<EventRegistration>
        {
            StatusCode = HttpStatusCode.OK,
            Message = $"Got event registration '{registration.Id}'",
            Data = registration
        };
    }

    #endregion

    #region Update

    public async Task<Response<Event>> UpdateEvent(Event @event, HttpRequest request)
    {
        var dbEvent = await _context.Events.AsNoTracking().FirstOrDefaultAsync(x => x.Id == @event.Id) ??
            throw new EventNotFoundException($"Event '{@event.Id}' not found");

        var oldData = JsonConvert.SerializeObject(dbEvent);
        @event.Updated = DateTimeOffset.UtcNow;
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
        var dbPosition = await _context.EventsPositions.AsNoTracking().FirstOrDefaultAsync(x => x.Id == position.Id) ??
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

    public async Task<Response<IList<EventRegistration>>> AssignReliefPositions(int eventId, HttpRequest request)
    {
        var @event = await _context.Events
            .Include(x => x.Positions)
            .FirstOrDefaultAsync(x => x.Id == eventId) ??
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

            await _loggingService.AddWebsiteLog(request, $"Asigned relief position '{entry.Id}'", oldData, newData);
            await _emailService.SendEventPositionAssigned(user.Email, eventId, @event.Name, "Relief", entry.Start, entry.End);
        }

        return new Response<IList<EventRegistration>>
        {
            StatusCode = HttpStatusCode.OK,
            Message = $"Assigned {registrations.Count} users as relief for event {eventId}",
            Data = registrations
        };
    }

    public async Task<Response<EventRegistration>> AssignEventPosition(int registrationId, int positionId, HttpRequest request)
    {
        var registration = await _context.EventsRegistrations
            .Include(x => x.Event)
            .Include(x => x.User)
            .FirstOrDefaultAsync(x => x.Id == registrationId) ??
            throw new EventRegistrationNotFoundException($"Event registration '{registrationId}' not found");
        if (registration.User == null)
            throw new UserNotFoundException($"User '{registration.UserId}' not found");
        if (registration.Event == null)
            throw new EventNotFoundException($"Event '{registration.EventId}' not found");

        var position = await _context.EventsPositions.FindAsync(positionId) ??
            throw new EventPositionNotFoundException($"Event position '{positionId}' not found");

        var oldData = JsonConvert.SerializeObject(registration);
        registration.Status = EventRegistrationStatus.Assigned;
        await _context.SaveChangesAsync();
        var newData = JsonConvert.SerializeObject(registration);

        await _loggingService.AddWebsiteLog(request, $"Assigned event position '{registrationId}'", oldData, newData);
        await _emailService.SendEventPositionAssigned(registration.User.Email, registration.EventId, registration.Event.Name, position.Name, registration.Start, registration.End);

        return new Response<EventRegistration>
        {
            StatusCode = HttpStatusCode.OK,
            Message = $"Assigned registration '{registrationId}' as {position.Name} for event {registration.Event.Id}",
            Data = registration
        };
    }

    public async Task<Response<EventRegistration>> UnAssignEventPosition(int registrationId, int positionId, HttpRequest request)
    {
        var registration = await _context.EventsRegistrations
            .Include(x => x.Event)
            .Include(x => x.User)
            .FirstOrDefaultAsync(x => x.Id == registrationId) ??
            throw new EventRegistrationNotFoundException($"Event registration '{registrationId}' not found");
        if (registration.User == null)
            throw new UserNotFoundException($"User '{registration.UserId}' not found");
        if (registration.Event == null)
            throw new EventNotFoundException($"Event '{registration.EventId}' not found");

        var position = await _context.EventsPositions.FindAsync(positionId) ??
            throw new EventPositionNotFoundException($"Event position '{positionId}' not found");

        var oldData = JsonConvert.SerializeObject(registration);
        registration.Status = EventRegistrationStatus.Pending;
        await _context.SaveChangesAsync();
        var newData = JsonConvert.SerializeObject(registration);

        await _loggingService.AddWebsiteLog(request, $"Unassigned event position '{registrationId}'", oldData, newData);
        await _emailService.SendEventPositionUnAssigned(registration.User.Email, registration.EventId, registration.Event.Name, position.Name);

        return new Response<EventRegistration>
        {
            StatusCode = HttpStatusCode.OK,
            Message = $"Unassigned registration '{registrationId}' as {position.Name} for event {registration.Event.Id}",
            Data = registration
        };
    }

    #endregion

    #region Delete

    public async Task<Response<Event>> DeleteEvent(int eventId, HttpRequest request)
    {
        var @event = await _context.Events
            .Include(x => x.Positions)
            .FirstOrDefaultAsync(x => x.Id == eventId) ??
            throw new EventNotFoundException($"Event '{eventId}' not found");

        var oldData = JsonConvert.SerializeObject(@event);
        var result = _context.Events.Remove(@event);
        await _context.SaveChangesAsync();

        await _loggingService.AddWebsiteLog(request, $"Deleted event '{eventId}'", oldData, string.Empty);
        return new Response<Event>
        {
            StatusCode = HttpStatusCode.OK,
            Message = $"Event '{eventId}' deleted",
            Data = result.Entity
        };
    }

    public async Task<Response<EventPosition>> DeleteEventPosition(int positionId, HttpRequest request)
    {
        var position = await _context.EventsPositions.FindAsync(positionId) ??
            throw new EventPositionNotFoundException($"Event position '{positionId}' not found");

        var oldData = JsonConvert.SerializeObject(position);
        var result = _context.EventsPositions.Remove(position);
        await _context.SaveChangesAsync();

        await _loggingService.AddWebsiteLog(request, $"Deleted event position '{positionId}'", oldData, string.Empty);
        return new Response<EventPosition>
        {
            StatusCode = HttpStatusCode.OK,
            Message = $"Event position '{positionId}' deleted",
            Data = result.Entity
        };
    }

    public async Task<Response<EventRegistration>> DeleteEventRegistration(int eventId, HttpRequest request)
    {
        var registration = await request.HttpContext.GetUserEventRegistration(_context, eventId) ??
            throw new EventRegistrationNotFoundException($"Event registration not found");

        var oldData = JsonConvert.SerializeObject(registration);
        var result = _context.EventsRegistrations.Remove(registration);
        await _context.SaveChangesAsync();

        await _loggingService.AddWebsiteLog(request, $"Deleted event registration '{registration.Id}'", oldData, string.Empty);
        return new Response<EventRegistration>
        {
            StatusCode = HttpStatusCode.OK,
            Message = $"Event registration '{registration.Id}' deleted",
            Data = result.Entity
        };
    }

    #endregion
}
