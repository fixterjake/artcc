using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using ZDC.Server.Services.Interfaces;
using ZDC.Shared.Dtos;
using ZDC.Shared.Models;

namespace ZDC.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EventsController : ControllerBase
{
    private readonly IEmailService _emailService;
    private readonly ILoggingService _loggingService;
    private readonly IValidator<Event> _eventValidator;
    private readonly IValidator<EventPosition> _eventPositionValidator;
    private readonly IValidator<EventRegistration> _eventRegistrationValidator;

    public EventsController(IEmailService emailService, ILoggingService loggingService, IValidator<Event> eventValidator,
        IValidator<EventPosition> eventPositionValidator, IValidator<EventRegistration> eventRegistrationValidator)
    {
        _emailService = emailService;
        _loggingService = loggingService;
        _eventValidator = eventValidator;
        _eventPositionValidator = eventPositionValidator;
        _eventRegistrationValidator = eventRegistrationValidator;
    }

    [HttpGet]
    public async Task<ActionResult<Response<bool>>> CreateEventRegistration()
    {
        try
        {
            return Ok(await _emailService.SendVisitRequestAccepted("fixterjake@gmail.com"));
        }
        catch (Exception ex)
        {
            return await _loggingService.AddDebugLog(Request, nameof(CreateEventRegistration), ex.Message, ex.StackTrace ?? "N/A");
        }
    }
}