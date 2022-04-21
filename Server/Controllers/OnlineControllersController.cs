using Microsoft.AspNetCore.Mvc;
using Sentry;
using Swashbuckle.AspNetCore.Annotations;
using ZDC.Server.Extensions;
using ZDC.Server.Repositories.Interfaces;
using ZDC.Shared.Dtos;
using ZDC.Shared.Models;

namespace ZDC.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OnlineControllersController : ControllerBase
{
    private readonly IOnlineControllerRepository _onlineControllerRepository;
    private readonly IHub _sentryHub;

    public OnlineControllersController(IOnlineControllerRepository onlineControllerRepository, IHub sentryHub)
    {
        _onlineControllerRepository = onlineControllerRepository;
        _sentryHub = sentryHub;
    }

    [HttpGet]
    [SwaggerResponse(200, "Got online controllers", typeof(Response<IList<OnlineController>>))]
    [SwaggerResponse(400, "An error occurred")]
    public async Task<ActionResult<Response<IList<OnlineController>>>> GetOnlineControllers()
    {
        try
        {
            return Ok(await _onlineControllerRepository.GetOnlineControllers());
        }
        catch (Exception ex)
        {
            return _sentryHub.CaptureException(ex).ReturnActionResult();
        }
    }
}
