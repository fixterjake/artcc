using ZDC.Shared.Dtos;
using ZDC.Shared.Models;

namespace ZDC.Server.Repositories.Interfaces;

public interface IEventRepository
{
    /// <summary>
    /// Create event
    /// </summary>
    /// <param name="event">Event to create</param>
    /// <param name="request">Raw http request</param>
    /// <returns>Created event</returns>
    Task<Response<Event>> CreateEvent(Event @event, HttpRequest request);

    /// <summary>
    /// Create event position
    /// </summary>
    /// <param name="position">Position to create</param>
    /// <param name="eventId">Event id</param>
    /// <param name="request">Raw http request</param>
    /// <exception cref="Shared.EventNotFoundException">Event not found</exception>
    /// <returns>Created event position</returns>
    Task<Response<EventPosition>> CreateEventPosition(EventPosition position, int eventId, HttpRequest request);

    /// <summary>
    /// Create event position
    /// </summary>
    /// <param name="registration">Registration to create</param>
    /// <param name="request">Raw http request</param>
    /// <exception cref="Shared.UserNotFoundException">User not found</exception>
    /// <exception cref="Shared.EventNotFoundException">Event not found</exception>
    /// <returns>Created registration</returns>
    Task<Response<EventRegistration>> CreateEventRegistration(EventRegistration registration, HttpRequest request);

    /// <summary>
    /// Get events
    /// </summary>
    /// <param name="skip">Number to skip</param>
    /// <param name="take">Number to take</param>
    /// <param name="request">Raw http request</param>
    /// <returns>Events</returns>
    Task<ResponsePaging<IList<Event>>> GetEvents(int skip, int take, HttpRequest request);

    /// <summary>
    /// Get event by id
    /// </summary>
    /// <param name="eventId">Event id</param>
    /// <param name="request">Raw http request</param>
    /// <exception cref="Shared.EventNotFoundException">Event not found</exception>
    /// <returns>Event if found</returns>
    Task<Response<Event>> GetEvent(int eventId, HttpRequest request);

    /// <summary>
    /// Get event registrations of event
    /// </summary>
    /// <param name="eventId">Event id</param>
    /// <exception cref="Shared.EventNotFoundException">Event not found</exception>
    /// <returns>Event registrations</returns>
    Task<Response<IList<EventRegistration>>> GetEventRegistrations(int eventId);

    /// <summary>
    /// Get a users event registration
    /// </summary>
    /// <param name="eventId">Event id to get registration</param>
    /// <param name="request">Raw http request</param>
    /// <exception cref="Shared.EventRegistrationNotFoundException">Event registration not found</exception>
    /// <returns>Event registration if found</returns>
    Task<Response<EventRegistration>> GetUserEventRegistration(int eventId, HttpRequest request);

    /// <summary>
    /// Update an event
    /// </summary>
    /// <param name="event">Updated event</param>
    /// <param name="request">Raw http request</param>
    /// <exception cref="Shared.EventNotFoundException">Event not found</exception>
    /// <returns>Updated event</returns>
    Task<Response<Event>> UpdateEvent(Event @event, HttpRequest request);

    /// <summary>
    /// Update event position
    /// </summary>
    /// <param name="position">Updated event position</param>
    /// <param name="request">Raw http request</param>
    /// <exception cref="Shared.EventPositionNotFoundException">Event position not found</exception>
    /// <returns>Updated event position</returns>
    Task<Response<EventPosition>> UpdateEventPosition(EventPosition position, HttpRequest request);

    /// <summary>
    /// Assign relief positions
    /// </summary>
    /// <param name="eventId">Event id to assign relief</param>
    /// <param name="request">Raw http request</param>
    /// <exception cref="Shared.EventNotFoundException">Event not found</exception>
    /// <returns>List of assigned registrations</returns>
    Task<Response<IList<EventRegistration>>> AssignReliefPositions(int eventId, HttpRequest request);

    /// <summary>
    /// Assign an event position
    /// </summary>
    /// <param name="registrationId">Registration id</param>
    /// <param name="positionId">Position id</param>
    /// <param name="request">Raw http request</param>
    /// <exception cref="Shared.EventRegistrationNotFoundException">Event registration not found</exception>
    /// <exception cref="Shared.EventPositionNotFoundException">Event position not found</exception>
    /// <exception cref="Shared.EventNotFoundException">Event not found</exception>
    /// <exception cref="Shared.UserNotFoundException">User not found</exception>
    /// <returns>Assigned position</returns>
    Task<Response<EventRegistration>> AssignEventPosition(int registrationId, int positionId, HttpRequest request);

    /// <summary>
    /// Unassign an event position
    /// </summary>
    /// <param name="registrationId">Registration id</param>
    /// <param name="positionId">Position id</param>
    /// <param name="request">Raw http reqest</param>
    /// <exception cref="Shared.EventRegistrationNotFoundException">Event registration not found</exception>
    /// <exception cref="Shared.EventPositionNotFoundException">Event position not found</exception>
    /// <exception cref="Shared.EventNotFoundException">Event not found</exception>
    /// <exception cref="Shared.UserNotFoundException">User not found</exception>
    /// <returns>Unassigned event registration</returns>
    Task<Response<EventRegistration>> UnAssignEventPosition(int registrationId, int positionId, HttpRequest request);

    /// <summary>
    /// Delete event
    /// </summary>
    /// <param name="eventId">Event id</param>
    /// <param name="request">Raw http request</param>
    /// <exception cref="Shared.EventNotFoundException">Event not found</exception>
    /// <returns>Deleted event</returns>
    Task<Response<Event>> DeleteEvent(int eventId, HttpRequest request);

    /// <summary>
    /// Delete event position
    /// </summary>
    /// <param name="positionId">Position id</param>
    /// <param name="request">Raw http request</param>
    /// <exception cref="Shared.EventPositionNotFoundException">Event position not found</exception>
    /// <returns>Deleted position</returns>
    Task<Response<EventPosition>> DeleteEventPosition(int positionId, HttpRequest request);

    /// <summary>
    /// Delete event registration
    /// </summary>
    /// <param name="eventId">Event id</param>
    /// <param name="request">Raw http request</param>
    /// <exception cref="Shared.EventRegistrationNotFoundException">Event registration not found</exception>
    /// <returns>Deleted registration</returns>
    Task<Response<EventRegistration>> DeleteEventRegistration(int eventId, HttpRequest request);
}