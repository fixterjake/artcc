using System.Net;
using Microsoft.AspNetCore.Mvc;
using Sentry;
using Swashbuckle.AspNetCore.Annotations;
using ZDC.Server.Extensions;
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
    private readonly IHub _sentryHub;

    public ControllerLogsController(IControllerLogRepository controllerLogRepository, IHub sentryHub)
    {
        _controllerLogRepository = controllerLogRepository;
        _sentryHub = sentryHub;
    }

    [HttpGet("{userId:int}")]
    [SwaggerResponse(200, "Got user controller logs", typeof(ResponsePaging<IList<ControllerLogDto>>))]
    [SwaggerResponse(404, "User not found")]
    [SwaggerResponse(400, "An error occurred")]
    public async Task<ActionResult<ResponsePaging<IList<ControllerLogDto>>>> GetUserControllerLogs(int userId, int skip = 0, int take = 10)
    {
        try
        {
            return Ok(await _controllerLogRepository.GetUserControllerLogs(userId, skip, take));
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
}