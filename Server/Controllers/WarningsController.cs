using Microsoft.AspNetCore.Authorization;
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
public class WarningsController : ControllerBase
{
    private readonly IWarningRepository _warningRepository;
    private readonly IHub _sentryHub;

    public WarningsController(IWarningRepository warningRepository, IHub sentryHub)
    {
        _warningRepository = warningRepository;
        _sentryHub = sentryHub;
    }

    #region Create

    [HttpPost]
    [Authorize(Policy = "IsSeniorStaff")]
    [SwaggerResponse(201, "Created warning", typeof(Response<Warning>))]
    [SwaggerResponse(400, "An error occurred")]
    [SwaggerResponse(401, "Unauthorized")]
    public async Task<ActionResult<Response<Warning>>> CreateWarning(Warning warning)
    {
        try
        {
            return Ok(await _warningRepository.CreateWarning(warning, Request));
        }
        catch (Exception ex)
        {
            return _sentryHub.CaptureException(ex).ReturnActionResult();
        }
    }

    [HttpPost("multiple")]
    [Authorize(Policy = "IsSeniorStaff")]
    [SwaggerResponse(201, "Created warnings", typeof(Response<IList<Warning>>))]
    [SwaggerResponse(400, "An error occurred")]
    [SwaggerResponse(401, "Unauthorized")]
    public async Task<ActionResult<Response<IList<Warning>>>> CreateWarnings(IList<Warning> warnings)
    {
        try
        {
            return Ok(await _warningRepository.CreateWarnings(warnings, Request));
        }
        catch (Exception ex)
        {
            return _sentryHub.CaptureException(ex).ReturnActionResult();
        }
    }

    #endregion

    #region Read

    [HttpGet("audit")]
    [Authorize(Policy = "IsSeniorStaff")]
    [SwaggerResponse(200, "Audited controllers", typeof(Response<IList<AuditDto>>))]
    [SwaggerResponse(400, "An error occurred")]
    [SwaggerResponse(401, "Unauthorized")]
    public async Task<ActionResult<Response<IList<AuditDto>>>> AuditControllers()
    {
        try
        {
            return Ok(await _warningRepository.AuditControllers());
        }
        catch (Exception ex)
        {
            return _sentryHub.CaptureException(ex).ReturnActionResult();
        }
    }

    [HttpGet]
    [Authorize(Policy = "IsSeniorStaff")]
    [SwaggerResponse(200, "Got warnings", typeof(Response<IList<Warning>>))]
    [SwaggerResponse(400, "An error occurred")]
    [SwaggerResponse(401, "Unauthorized")]
    public async Task<ActionResult<Response<IList<Warning>>>> GetWarnings(int month, int year)
    {
        try
        {
            return Ok(await _warningRepository.GetWarnings(month, year));
        }
        catch (Exception ex)
        {
            return _sentryHub.CaptureException(ex).ReturnActionResult();
        }
    }

    #endregion

    #region Delete

    [HttpDelete("{id:int}")]
    [Authorize(Policy = "IsSeniorStaff")]
    [SwaggerResponse(200, "Got warnings", typeof(Response<Warning>))]
    [SwaggerResponse(400, "An error occurred")]
    [SwaggerResponse(401, "Unauthorized")]
    public async Task<ActionResult<Response<Warning>>> DeleteWarning(int warningId)
    {
        try
        {
            return Ok(await _warningRepository.DeleteWarning(warningId, Request));
        }
        catch (WarningNotFoundException ex)
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
