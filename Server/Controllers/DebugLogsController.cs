using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using ZDC.Server.Repositories.Interfaces;
using ZDC.Server.Services.Interfaces;
using ZDC.Shared.Dtos;
using ZDC.Shared.Models;

namespace ZDC.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DebugLogsController : ControllerBase
{
    private readonly IDebugLogRepository _debugLogRepository;
    private readonly ILoggingService _loggingService;

    public DebugLogsController(IDebugLogRepository debugLogRepository, ILoggingService loggingService)
    {
        _debugLogRepository = debugLogRepository;
        _loggingService = loggingService;
    }

    [HttpGet]
    [SwaggerResponse(200, "Got all debug logs", typeof(Response<IList<DebugLog>>))]
    [SwaggerResponse(400, "An error occurred")]
    public async Task<ActionResult<Response<IList<DebugLog>>>> GetDebugLogs()
    {
        try
        {
            return Ok(await _debugLogRepository.GetDebugLogs());
        }
        catch (Exception ex)
        {
            return await _loggingService.AddDebugLog(Request, nameof(GetDebugLogs), ex.Message, ex.StackTrace);
        }
    }
}