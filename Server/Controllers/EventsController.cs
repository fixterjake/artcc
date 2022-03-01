using Microsoft.AspNetCore.Mvc;
using ZDC.Server.Services.Interfaces;
using ZDC.Shared.Dtos;

namespace ZDC.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EventsController : ControllerBase
{
    private readonly IEmailService _emailService;
    private readonly ILoggingService _loggingService;

    public EventsController(IEmailService emailService, ILoggingService loggingService)
    {
        _emailService = emailService;
        _loggingService = loggingService;
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
            return await _loggingService.AddDebugLog(Request, nameof(CreateEventRegistration), ex.Message,
                ex.StackTrace);
        }
    }
}