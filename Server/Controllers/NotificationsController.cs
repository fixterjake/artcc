using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Sentry;
using Swashbuckle.AspNetCore.Annotations;
using System.Net;
using ZDC.Server.Extensions;
using ZDC.Server.Repositories.Interfaces;
using ZDC.Shared;
using ZDC.Shared.Dtos;
using ZDC.Shared.Models;

namespace ZDC.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class NotificationsController : ControllerBase
{
    private readonly INotificationRepository _notificationRepository;
    private readonly IHub _sentryHub;

    public NotificationsController(INotificationRepository notificationRepository, IHub sentryHub)
    {
        _notificationRepository = notificationRepository;
        _sentryHub = sentryHub;
    }

    [HttpGet]
    [SwaggerResponse(200, "Got notifications", typeof(ResponsePaging<IList<Notification>>))]
    [SwaggerResponse(404, "Airport not found")]
    [SwaggerResponse(400, "An error occurred")]
    public async Task<ActionResult<ResponsePaging<IList<Notification>>>> GetNotifications(int skip = 0, int take = 10)
    {
        try
        {
            return Ok(await _notificationRepository.GetNotifications(skip, take, Request));
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
