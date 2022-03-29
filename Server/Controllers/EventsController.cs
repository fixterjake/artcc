using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Sentry;
using Swashbuckle.AspNetCore.Annotations;
using System.Net;
using ZDC.Server.Extensions;
using ZDC.Server.Repositories.Interfaces;
using ZDC.Server.Services.Interfaces;
using ZDC.Shared;
using ZDC.Shared.Dtos;
using ZDC.Shared.Models;

namespace ZDC.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EventsController : ControllerBase
{
    private readonly IEventRepository _eventRepository;
    private readonly IValidator<Event> _eventValidator;
    private readonly IValidator<EventPosition> _eventPositionValidator;
    private readonly IValidator<EventRegistration> _eventRegistrationValidator;
    private readonly IHub _sentryHub;

    public EventsController(IEventRepository eventRepository, IValidator<Event> eventValidator,
        IValidator<EventPosition> eventPositionValidator, IValidator<EventRegistration> eventRegistrationValidator, IHub sentryHub)
    {
        _eventRepository = eventRepository;
        _eventValidator = eventValidator;
        _eventPositionValidator = eventPositionValidator;
        _eventRegistrationValidator = eventRegistrationValidator;
        _sentryHub = sentryHub;
    }

    #region Create

    [HttpPost]
    // todo auth
    [SwaggerResponse(200, "Created event", typeof(Response<Event>))]
    [SwaggerResponse(400, "An error occurred")]
    public async Task<ActionResult<Response<Event>>> CreateEvent([FromBody] Event @event)
    {
        try
        {
            var result = _eventValidator.Validate(@event);
            if (!result.IsValid)
            {
                return BadRequest(new Response<IList<ValidationFailure>>
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    Message = "Validation error",
                    Data = result.Errors
                });
            }
            return Ok(await _eventRepository.CreateEvent(@event, Request));
        }
        catch (Exception ex)
        {
            return _sentryHub.CaptureException(ex).ReturnActionResult();
        }
    }

    [HttpPost("{eventId:int}/position")]
    // todo auth
    [SwaggerResponse(200, "Created event position", typeof(Response<EventRegistration>))]
    [SwaggerResponse(404, "Not found")]
    [SwaggerResponse(400, "An error occurred")]
    public async Task<ActionResult<Response<EventPosition>>> CreateEventPosition(int eventId, [FromBody] EventPosition position)
    {
        try
        {
            var result = _eventPositionValidator.Validate(position);
            if (!result.IsValid)
            {
                return BadRequest(new Response<IList<ValidationFailure>>
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    Message = "Validation error",
                    Data = result.Errors
                });
            }
            return Ok(await _eventRepository.CreateEventPosition(position, eventId, Request));
        }
        catch (EventNotFoundException ex)
        {
            return NotFound(new Response<string>
            {
                StatusCode = HttpStatusCode.NotFound,
                Message = ex.Message,
                Data = string.Empty
            });
        }
        catch (Exception ex)
        {
            return _sentryHub.CaptureException(ex).ReturnActionResult();
        }
    }

    [HttpPost("registration")]
    // todo auth
    [SwaggerResponse(200, "Created event registration", typeof(Response<EventRegistration>))]
    [SwaggerResponse(404, "Not found")]
    [SwaggerResponse(400, "An error occurred")]
    public async Task<ActionResult<Response<Event>>> CreateEventRegistration([FromBody] EventRegistration registration)
    {
        try
        {
            var result = _eventRegistrationValidator.Validate(registration);
            if (!result.IsValid)
            {
                return BadRequest(new Response<IList<ValidationFailure>>
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    Message = "Validation error",
                    Data = result.Errors
                });
            }
            return Ok(await _eventRepository.CreateEventRegistration(registration, Request));
        }
        catch (UserNotFoundException ex)
        {
            return NotFound(new Response<string>
            {
                StatusCode = HttpStatusCode.NotFound,
                Message = ex.Message,
                Data = string.Empty
            });
        }
        catch (EventNotFoundException ex)
        {
            return NotFound(new Response<string>
            {
                StatusCode = HttpStatusCode.NotFound,
                Message = ex.Message,
                Data = string.Empty
            });
        }
        catch (Exception ex)
        {
            return _sentryHub.CaptureException(ex).ReturnActionResult();
        }
    }

    #endregion

    #region Read

    [HttpGet]
    [SwaggerResponse(200, "Got all events", typeof(Response<IList<Event>>))]
    [SwaggerResponse(400, "An error occurred")]
    public async Task<ActionResult<Response<IList<Event>>>> GetEvents()
    {
        try
        {
            return Ok(await _eventRepository.GetEvents(Request));
        }
        catch (Exception ex)
        {
            return _sentryHub.CaptureException(ex).ReturnActionResult();
        }
    }

    [HttpGet("{eventId:int}")]
    [SwaggerResponse(200, "Got event", typeof(Response<Event>))]
    [SwaggerResponse(404, "Not found")]
    [SwaggerResponse(400, "An error occurred")]
    public async Task<ActionResult<Response<IList<Event>>>> GetEvent(int eventId)
    {
        try
        {
            return Ok(await _eventRepository.GetEvent(eventId, Request));
        }
        catch (EventNotFoundException ex)
        {
            return NotFound(new Response<string>
            {
                StatusCode = HttpStatusCode.NotFound,
                Message = ex.Message,
                Data = string.Empty
            });
        }
        catch (Exception ex)
        {
            return _sentryHub.CaptureException(ex).ReturnActionResult();
        }
    }

    [HttpGet("registrations/{eventId:int}")]
    // todo auth
    [SwaggerResponse(200, "Got event registrations", typeof(Response<IList<EventRegistration>>))]
    [SwaggerResponse(404, "Not found")]
    [SwaggerResponse(400, "An error occurred")]
    public async Task<ActionResult<Response<IList<Event>>>> GetEventRegistrations(int eventId)
    {
        try
        {
            return Ok(await _eventRepository.GetEventRegistrations(eventId));
        }
        catch (EventNotFoundException ex)
        {
            return NotFound(new Response<string>
            {
                StatusCode = HttpStatusCode.NotFound,
                Message = ex.Message,
                Data = string.Empty
            });
        }
        catch (Exception ex)
        {
            return _sentryHub.CaptureException(ex).ReturnActionResult();
        }
    }


    [HttpGet("registration/{eventId:int}")]
    // todo auth
    [SwaggerResponse(200, "Got event registration", typeof(Response<EventRegistration>))]
    [SwaggerResponse(404, "Not found")]
    [SwaggerResponse(400, "An error occurred")]
    public async Task<ActionResult<Response<IList<Event>>>> GetEventRegistration(int eventId)
    {
        try
        {
            return Ok(await _eventRepository.GetUserEventRegistration(eventId, Request));
        }
        catch (EventRegistrationNotFoundException ex)
        {
            return NotFound(new Response<string>
            {
                StatusCode = HttpStatusCode.NotFound,
                Message = ex.Message,
                Data = string.Empty
            });
        }
        catch (Exception ex)
        {
            return _sentryHub.CaptureException(ex).ReturnActionResult();
        }
    }

    #endregion

    #region Update

    [HttpPut]
    // todo auth
    [SwaggerResponse(200, "Updated event", typeof(Response<Event>))]
    [SwaggerResponse(404, "Not found")]
    [SwaggerResponse(400, "An error occurred")]
    public async Task<ActionResult<Response<Event>>> UpdateEvent([FromBody] Event @event)
    {
        try
        {
            var result = _eventValidator.Validate(@event);
            if (!result.IsValid)
            {
                return BadRequest(new Response<IList<ValidationFailure>>
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    Message = "Validation error",
                    Data = result.Errors
                });
            }
            return Ok(await _eventRepository.UpdateEvent(@event, Request));
        }
        catch (EventNotFoundException ex)
        {
            return NotFound(new Response<string>
            {
                StatusCode = HttpStatusCode.NotFound,
                Message = ex.Message,
                Data = string.Empty
            });
        }
        catch (Exception ex)
        {
            return _sentryHub.CaptureException(ex).ReturnActionResult();
        }
    }

    [HttpPut("position")]
    // todo auth
    [SwaggerResponse(200, "Updated event position", typeof(Response<EventPosition>))]
    [SwaggerResponse(404, "Not found")]
    [SwaggerResponse(400, "An error occurred")]
    public async Task<ActionResult<Response<Event>>> UpdateEventPosition([FromBody] EventPosition position)
    {
        try
        {
            var result = _eventPositionValidator.Validate(position);
            if (!result.IsValid)
            {
                return BadRequest(new Response<IList<ValidationFailure>>
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    Message = "Validation error",
                    Data = result.Errors
                });
            }
            return Ok(await _eventRepository.UpdateEventPosition(position, Request));
        }
        catch (EventPositionNotFoundException ex)
        {
            return NotFound(new Response<string>
            {
                StatusCode = HttpStatusCode.NotFound,
                Message = ex.Message,
                Data = string.Empty
            });
        }
        catch (Exception ex)
        {
            return _sentryHub.CaptureException(ex).ReturnActionResult();
        }
    }

    [HttpPut("relief/{eventId:int}")]
    // todo auth
    [SwaggerResponse(200, "Assigned relief positions", typeof(Response<EventPosition>))]
    [SwaggerResponse(404, "Not found")]
    [SwaggerResponse(400, "An error occurred")]
    public async Task<ActionResult<Response<IList<EventRegistration>>>> AssignReliefPositions(int eventId)
    {
        try
        {
            return Ok(await _eventRepository.AssignReliefPositions(eventId, Request));
        }
        catch (EventNotFoundException ex)
        {
            return NotFound(new Response<string>
            {
                StatusCode = HttpStatusCode.NotFound,
                Message = ex.Message,
                Data = string.Empty
            });
        }
        catch (Exception ex)
        {
            return _sentryHub.CaptureException(ex).ReturnActionResult();
        }
    }

    [HttpPut("assign/{registrationId:int}/{positionId:int}")]
    // todo auth
    [SwaggerResponse(200, "Assigned event position", typeof(Response<EventRegistration>))]
    [SwaggerResponse(404, "Not found")]
    [SwaggerResponse(400, "An error occurred")]
    public async Task<ActionResult<Response<EventRegistration>>> AssignEventPosition(int registrationId, int positionId)
    {
        try
        {
            return Ok(await _eventRepository.AssignEventPosition(registrationId, positionId, Request));
        }
        catch (EventRegistrationNotFoundException ex)
        {
            return NotFound(new Response<string>
            {
                StatusCode = HttpStatusCode.NotFound,
                Message = ex.Message,
                Data = string.Empty
            });
        }
        catch (EventNotFoundException ex)
        {
            return NotFound(new Response<string>
            {
                StatusCode = HttpStatusCode.NotFound,
                Message = ex.Message,
                Data = string.Empty
            });
        }
        catch (EventPositionNotFoundException ex)
        {
            return NotFound(new Response<string>
            {
                StatusCode = HttpStatusCode.NotFound,
                Message = ex.Message,
                Data = string.Empty
            });
        }
        catch (UserNotFoundException ex)
        {
            return NotFound(new Response<string>
            {
                StatusCode = HttpStatusCode.NotFound,
                Message = ex.Message,
                Data = string.Empty
            });
        }
        catch (Exception ex)
        {
            return _sentryHub.CaptureException(ex).ReturnActionResult();
        }
    }

    [HttpPut("unassign/{registrationId:int}/{positionId:int}")]
    // todo auth
    [SwaggerResponse(200, "Unassigned event position", typeof(Response<EventRegistration>))]
    [SwaggerResponse(404, "Not found")]
    [SwaggerResponse(400, "An error occurred")]
    public async Task<ActionResult<Response<EventRegistration>>> UnassignEventPosition(int registrationId, int positionId)
    {
        try
        {
            return Ok(await _eventRepository.UnAssignEventPosition(registrationId, positionId, Request));
        }
        catch (EventRegistrationNotFoundException ex)
        {
            return NotFound(new Response<string>
            {
                StatusCode = HttpStatusCode.NotFound,
                Message = ex.Message,
                Data = string.Empty
            });
        }
        catch (EventNotFoundException ex)
        {
            return NotFound(new Response<string>
            {
                StatusCode = HttpStatusCode.NotFound,
                Message = ex.Message,
                Data = string.Empty
            });
        }
        catch (EventPositionNotFoundException ex)
        {
            return NotFound(new Response<string>
            {
                StatusCode = HttpStatusCode.NotFound,
                Message = ex.Message,
                Data = string.Empty
            });
        }
        catch (UserNotFoundException ex)
        {
            return NotFound(new Response<string>
            {
                StatusCode = HttpStatusCode.NotFound,
                Message = ex.Message,
                Data = string.Empty
            });
        }
        catch (Exception ex)
        {
            return _sentryHub.CaptureException(ex).ReturnActionResult();
        }
    }

    #endregion

    #region Delete

    [HttpDelete("{eventId:int}")]
    // todo auth
    [SwaggerResponse(200, "Deleted event", typeof(Response<Event>))]
    [SwaggerResponse(404, "Not found")]
    [SwaggerResponse(400, "An error occurred")]
    public async Task<ActionResult<Response<Event>>> DeleteEvent(int eventId)
    {
        try
        {
            return Ok(await _eventRepository.DeleteEvent(eventId, Request));
        }
        catch (EventNotFoundException ex)
        {
            return NotFound(new Response<string>
            {
                StatusCode = HttpStatusCode.NotFound,
                Message = ex.Message,
                Data = string.Empty
            });
        }
        catch (Exception ex)
        {
            return _sentryHub.CaptureException(ex).ReturnActionResult();
        }
    }

    [HttpDelete("position/{positionId:int}")]
    // todo auth
    [SwaggerResponse(200, "Deleted event position", typeof(Response<EventPosition>))]
    [SwaggerResponse(404, "Not found")]
    [SwaggerResponse(400, "An error occurred")]
    public async Task<ActionResult<Response<Event>>> DeleteEventPosition(int positionId)
    {
        try
        {
            return Ok(await _eventRepository.DeleteEventPosition(positionId, Request));
        }
        catch (EventPositionNotFoundException ex)
        {
            return NotFound(new Response<string>
            {
                StatusCode = HttpStatusCode.NotFound,
                Message = ex.Message,
                Data = string.Empty
            });
        }
        catch (Exception ex)
        {
            return _sentryHub.CaptureException(ex).ReturnActionResult();
        }
    }

    [HttpDelete("registration/{eventId:int}")]
    // todo auth
    [SwaggerResponse(200, "Deleted event registration", typeof(Response<EventRegistration>))]
    [SwaggerResponse(404, "Not found")]
    [SwaggerResponse(400, "An error occurred")]
    public async Task<ActionResult<Response<Event>>> DeleteEventRegistration(int eventId)
    {
        try
        {
            return Ok(await _eventRepository.DeleteEventRegistration(eventId, Request));
        }
        catch (EventRegistrationNotFoundException ex)
        {
            return NotFound(new Response<string>
            {
                StatusCode = HttpStatusCode.NotFound,
                Message = ex.Message,
                Data = string.Empty
            });
        }
        catch (Exception ex)
        {
            return _sentryHub.CaptureException(ex).ReturnActionResult();
        }
    }

    #endregion
}