using ZDC.Shared.Dtos;
using ZDC.Shared.Models;

namespace ZDC.Server.Repositories.Interfaces;

public interface IEventRepository
{
    Task<Response<Event>> CreateEvent(Event @event, HttpRequest request);
    Task<Response<EventPosition>> CreateEventPosition(EventPosition position, HttpRequest request);
    Task<Response<EventRegistration>> CreateEventRegistration(EventRegistration registration, HttpRequest request);
    Task<Response<IList<Event>>> GetEvents();
    Task<Response<Event>> GetEvent(int id);
    Task<Response<IList<EventPosition>>> GetEventPositions(int id);
    Task<Response<IList<EventRegistration>>> GetEventRegistrations(int id);
    Task<Response<EventRegistration>> GetUserEventRegistration(int eventId, HttpRequest request);
    Task<Response<Event>> UpdateEvent(Event @event, HttpRequest request);
    Task<Response<EventPosition>> UpdateEventPosition(EventPosition position, HttpRequest request);
    Task<Response<EventRegistration>> UpdateEventRegistration(EventRegistration registration, HttpRequest request);
    Task<Response<IList<EventRegistration>>> AssignReliefPositions(int eventId, HttpRequest request);
    Task<Response<EventRegistration>> AssignEventPositions(int registrationId, int positionId, HttpRequest request);
    Task<Response<EventRegistration>> UnAssignEventPositions(int registrationId, int positionId, HttpRequest request);
    Task<Response<Event>> DeleteEvent(int id, HttpRequest request);
    Task<Response<EventPosition>> DeleteEventPosition(int id, HttpRequest request);
    Task<Response<EventRegistration>> DeleteEventRegistration(int id, HttpRequest request);
}