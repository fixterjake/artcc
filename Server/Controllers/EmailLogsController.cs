using Amazon.Auth.AccessControlPolicy;
using Microsoft.AspNetCore.Authorization;
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
public class EmailLogsController : ControllerBase
{
    private readonly IEmailLogRepository _emailLogRepository;
    private readonly IHub _sentryHub;

    public EmailLogsController(IEmailLogRepository emailLogRepository, IHub sentryHub)
    {
        _emailLogRepository = emailLogRepository;
        _sentryHub = sentryHub;
    }

    [HttpGet]
    [Authorize(Policy = "CanWeb")]
    [SwaggerResponse(200, "Got all email logs", typeof(ResponsePaging<IList<EmailLog>>))]
    [SwaggerResponse(400, "An error occurred")]
    [SwaggerResponse(401, "Unauthorized")]
    public async Task<ActionResult<ResponsePaging<IList<EmailLog>>>> GetEmailLogs(int skip = 0, int take = 10)
    {
        try
        {
            return Ok(await _emailLogRepository.GetEmailLogs(skip, take));
        }
        catch (Exception ex)
        {
            return _sentryHub.CaptureException(ex).ReturnActionResult();
        }
    }
}
