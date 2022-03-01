using System.Net;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using ZDC.Server.Repositories.Interfaces;
using ZDC.Server.Services.Interfaces;
using ZDC.Shared;
using ZDC.Shared.Dtos;
using ZDC.Shared.Models;

namespace ZDC.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ControllerLogsController : ControllerBase
{
    private readonly IControllerLogRepository _controllerLogRepository;
    private readonly ILoggingService _loggingService;

    public ControllerLogsController(IControllerLogRepository controllerLogRepository, ILoggingService loggingService)
    {
        _controllerLogRepository = controllerLogRepository;
        _loggingService = loggingService;
    }

    [HttpGet("{id:int}")]
    [SwaggerResponse(200, "Got user controller logs", typeof(Response<IList<ControllerLog>>))]
    [SwaggerResponse(404, "User not found")]
    [SwaggerResponse(400, "An error occurred")]
    public async Task<ActionResult<Response<IList<Comment>>>> GetUserControllerLogs(int id)
    {
        try
        {
            return Ok(await _controllerLogRepository.GetUserControllerLogs(id));
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
            return await _loggingService.AddDebugLog(Request, nameof(GetUserControllerLogs), ex.Message, ex.StackTrace);
        }
    }
}