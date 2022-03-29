using ZDC.Shared.Dtos;
using ZDC.Shared.Models;

namespace ZDC.Server.Repositories.Interfaces;

public interface IEventRepository
{
    Task<Response<Event>> CreateEvent(Event @event, HttpRequest request);
    Task<Response<EventPosition>> CreateEventPosition(EventPosition position, int eventId, HttpRequest request);
    Task<Response<EventRegistration>> CreateEventRegistration(EventRegistration registration, HttpRequest request);
    Task<Response<IList<Event>>> GetEvents(HttpRequest request);
    Task<Response<Event>> GetEvent(int eventId, HttpRequest request);
    Task<Response<IList<EventRegistration>>> GetEventRegistrations(int eventId);
    Task<Response<EventRegistration>> GetUserEventRegistration(int eventId, HttpRequest request);
    Task<Response<Event>> UpdateEvent(Event @event, HttpRequest request);
    Task<Response<EventPosition>> UpdateEventPosition(EventPosition position, HttpRequest request);
    Task<Response<IList<EventRegistration>>> AssignReliefPositions(int eventId, HttpRequest request);
    Task<Response<EventRegistration>> AssignEventPosition(int registrationId, int positionId, HttpRequest request);
    Task<Response<EventRegistration>> UnAssignEventPosition(int registrationId, int positionId, HttpRequest request);
    Task<Response<Event>> DeleteEvent(int eventId, HttpRequest request);
    Task<Response<EventPosition>> DeleteEventPosition(int positionId, HttpRequest request);
    Task<Response<EventRegistration>> DeleteEventRegistration(int eventId, HttpRequest request);
}