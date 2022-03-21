using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using ZDC.Server.Repositories.Interfaces;
using ZDC.Server.Services.Interfaces;
using ZDC.Shared.Dtos;
using ZDC.Shared.Models;

namespace ZDC.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EmailLogsController : ControllerBase
{
    private readonly IEmailLogRepository _emailLogRepository;
    private readonly ILoggingService _loggingService;

    public EmailLogsController(IEmailLogRepository emailLogRepository, ILoggingService loggingService)
    {
        _emailLogRepository = emailLogRepository;
        _loggingService = loggingService;
    }

    [HttpGet]
    [SwaggerResponse(200, "Got all email logs", typeof(Response<IList<EmailLog>>))]
    [SwaggerResponse(400, "An error occurred")]
    public async Task<ActionResult<IList<EmailLog>>> GetEmailLogs(int skip = 0, int take = 100)
    {
        try
        {
            return Ok(await _emailLogRepository.GetEmailLogs(skip, take));
        }
        catch (Exception ex)
        {
            return await _loggingService.AddDebugLog(Request, nameof(GetEmailLogs), ex.Message, ex.StackTrace ?? "N/A");
        }
    }
}
