using FluentValidation;
using FluentValidation.Results;
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
public class SoloCertsController : ControllerBase
{
    private readonly ISoloCertRepository _soloCertRepository;
    private readonly IValidator<SoloCert> _validator;
    private readonly IHub _sentryHub;

    public SoloCertsController(ISoloCertRepository soloCertRepository, IValidator<SoloCert> validator, IHub sentryHub)
    {
        _soloCertRepository = soloCertRepository;
        _validator = validator;
        _sentryHub = sentryHub;
    }

    #region Create

    [HttpPost]
    // todo auth
    [SwaggerResponse(200, "Created solo cert", typeof(Response<SoloCert>))]
    [SwaggerResponse(404, "User not found")]
    [SwaggerResponse(400, "An error occurred")]
    public async Task<ActionResult<Response<SoloCert>>> CreateSoloCert([FromBody] SoloCert soloCert)
    {
        try
        {
            var result = await _validator.ValidateAsync(soloCert);
            if (!result.IsValid)
            {
                return BadRequest(new Response<IList<ValidationFailure>>
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    Message = "Validation error",
                    Data = result.Errors
                });
            }
            return Ok(await _soloCertRepository.CreateSoloCert(soloCert, Request));
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
        catch (SoloCertExistsException ex)
        {
            return BadRequest(new Response<string>
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

    #region Read

    [HttpGet]
    // todo auth
    [SwaggerResponse(200, "Got all solo certs", typeof(Response<SoloCert>))]
    [SwaggerResponse(400, "An error occurred")]
    public async Task<ActionResult<Response<IList<SoloCert>>>> GetSoloCerts()
    {
        try
        {
            return Ok(await _soloCertRepository.GetSoloCerts());
        }
        catch (Exception ex)
        {
            return _sentryHub.CaptureException(ex).ReturnActionResult();
        }
    }

    [HttpGet("{userId:int}")]
    // todo auth
    [SwaggerResponse(200, "Got solo cert", typeof(Response<SoloCert>))]
    [SwaggerResponse(404, "User or solo cert not found")]
    [SwaggerResponse(400, "An error occurred")]
    public async Task<ActionResult<Response<SoloCert>>> GetSoloCert(int userId)
    {
        try
        {
            return Ok(await _soloCertRepository.GetSoloCert(userId));
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
        catch (SoloCertNotFoundException ex)
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

    #region Update

    [HttpPut("{soloCertId:int}/{end:DateTime}")]
    // todo auth
    [SwaggerResponse(200, "Updated solo cert", typeof(Response<SoloCert>))]
    [SwaggerResponse(404, "User or solo cert not found")]
    [SwaggerResponse(400, "An error occurred")]
    public async Task<ActionResult<Response<SoloCert>>> ExtendSoloCert(int soloCertId, DateTimeOffset end)
    {
        try
        {
            return Ok(await _soloCertRepository.ExtendSoloCert(soloCertId, end, Request));
        }
        catch (SoloCertNotFoundException ex)
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

    #region Delete

    [HttpDelete("{soloCertId:int}")]
    // todo auth
    [SwaggerResponse(200, "Deleted solo cert", typeof(Response<SoloCert>))]
    [SwaggerResponse(404, "Solo cert not found")]
    [SwaggerResponse(400, "An error occurred")]
    public async Task<ActionResult<Response<SoloCert>>> DeletSoloCert(int soloCertId)
    {
        try
        {
            return Ok(await _soloCertRepository.DeleteSoloCert(soloCertId, Request));
        }
        catch (SoloCertNotFoundException ex)
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
