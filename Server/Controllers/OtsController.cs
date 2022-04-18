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
public class OtsController : ControllerBase
{
    private readonly IOtsRepository _otsRepository;
    private readonly IValidator<Ots> _validator;
    private IHub _sentryHub;

    public OtsController(IOtsRepository otsRepository, IValidator<Ots> validator, IHub sentryHub)
    {
        _otsRepository = otsRepository;
        _validator = validator;
        _sentryHub = sentryHub;
    }

    #region Create

    [HttpPost]
    // todo auth
    [SwaggerResponse(200, "Created ots", typeof(Response<Ots>))]
    [SwaggerResponse(400, "An error occurred")]
    public async Task<ActionResult<Response<Ots>>> CreateOts([FromBody] Ots ots)
    {
        try
        {
            var result = await _validator.ValidateAsync(ots);
            if (!result.IsValid)
            {
                return BadRequest(new Response<IList<ValidationFailure>>
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    Message = "Validation error",
                    Data = result.Errors
                });
            }
            return Ok(await _otsRepository.CreateOts(ots, Request));
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

    #endregion

    #region Read

    [HttpGet]
    // todo auth
    [SwaggerResponse(200, "Got all ots's", typeof(Response<IList<Ots>>))]
    [SwaggerResponse(400, "An error occurred")]
    public async Task<ActionResult<Response<IList<Ots>>>> GetOts(int skip, int take, OtsStatus status)
    {
        try
        {
            return Ok(await _otsRepository.GetOts(skip, take, status));
        }
        catch (Exception ex)
        {
            return _sentryHub.CaptureException(ex).ReturnActionResult();
        }
    }

    [HttpGet("{otsId:int}")]
    // todo auth
    [SwaggerResponse(200, "Got ots", typeof(Response<Ots>))]
    [SwaggerResponse(404, "Ots not found")]
    [SwaggerResponse(400, "An error occurred")]
    public async Task<ActionResult<Response<Ots>>> GetOts(int otsId)
    {
        try
        {
            return Ok(await _otsRepository.GetOts(otsId));
        }
        catch (Exception ex)
        {
            return _sentryHub.CaptureException(ex).ReturnActionResult();
        }
    }

    #endregion

    #region Update

    [HttpPut]
    // todo auth
    [SwaggerResponse(200, "Updated ots", typeof(Response<Ots>))]
    [SwaggerResponse(404, "Ots not found")]
    [SwaggerResponse(400, "An error occurred")]
    public async Task<ActionResult<Response<Ots>>> UpdateOts([FromBody] Ots ots)
    {
        try
        {
            var result = await _validator.ValidateAsync(ots);
            if (!result.IsValid)
            {
                return BadRequest(new Response<IList<ValidationFailure>>
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    Message = "Validation error",
                    Data = result.Errors
                });
            }
            return Ok(await _otsRepository.UpdateOts(ots, Request));
        }
        catch (Exception ex)
        {
            return _sentryHub.CaptureException(ex).ReturnActionResult();
        }
    }

    #endregion

    #region Delete

    [HttpDelete("{otsId:int}")]
    // todo auth
    [SwaggerResponse(200, "Deleted ots", typeof(Response<Ots>))]
    [SwaggerResponse(404, "Ots not found")]
    [SwaggerResponse(400, "An error occurred")]
    public async Task<ActionResult<Response<Ots>>> DeleteOts(int otsId)
    {
        try
        {
            return Ok(await _otsRepository.DeleteOts(otsId, Request));
        }
        catch (Exception ex)
        {
            return _sentryHub.CaptureException(ex).ReturnActionResult();
        }
    }

    #endregion
}
