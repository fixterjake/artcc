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
public class LoasController : ControllerBase
{
    private readonly ILoaRepository _loaRepository;
    private readonly IValidator<Loa> _validator;
    private readonly IHub _sentryHub;

    public LoasController(ILoaRepository loaRepository, IValidator<Loa> validator, IHub sentryHub)
    {
        _loaRepository = loaRepository;
        _validator = validator;
        _sentryHub = sentryHub;
    }

    #region Create

    [HttpPost]
    // todo auth
    [SwaggerResponse(200, "Created loa", typeof(Response<Loa>))]
    [SwaggerResponse(400, "An error occurred")]
    public async Task<ActionResult<Response<Loa>>> CreateLoa([FromBody] Loa loa)
    {
        try
        {
            var result = await _validator.ValidateAsync(loa);
            if (!result.IsValid)
            {
                return BadRequest(new Response<IList<ValidationFailure>>
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    Message = "Validation error",
                    Data = result.Errors
                });
            }
            return Ok(await _loaRepository.CreateLoa(loa, Request));
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
    [SwaggerResponse(200, "Got loas", typeof(ResponsePaging<IList<Loa>>))]
    [SwaggerResponse(400, "An error occurred")]
    public async Task<ActionResult<ResponsePaging<IList<Loa>>>> GetLoas(int skip = 0, int take = 10)
    {
        try
        {
            return Ok(await _loaRepository.GetLoas(skip, take));
        }
        catch (Exception ex)
        {
            return _sentryHub.CaptureException(ex).ReturnActionResult();
        }
    }

    [HttpGet("{loaId:int}")]
    // todo auth
    [SwaggerResponse(200, "Got loa", typeof(Response<Loa>))]
    [SwaggerResponse(404, "Loa not found")]
    [SwaggerResponse(400, "An error occurred")]
    public async Task<ActionResult<Response<Loa>>> GetLoa(int loaId)
    {
        try
        {
            return Ok(await _loaRepository.GetLoa(loaId));
        }
        catch (LoaNotFoundException ex)
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

    [HttpPut]
    // todo auth
    [SwaggerResponse(200, "Updated loa", typeof(Response<Loa>))]
    [SwaggerResponse(404, "Loa not found")]
    [SwaggerResponse(400, "An error occurred")]
    public async Task<ActionResult<Response<Loa>>> UpdateLoa([FromBody] Loa loa)
    {
        try
        {
            return Ok(await _loaRepository.UpdateLoa(loa, Request));
        }
        catch (LoaNotFoundException ex)
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

    [HttpDelete("{loaId:int}")]
    // todo auth
    [SwaggerResponse(200, "Deleted loa", typeof(Response<Loa>))]
    [SwaggerResponse(404, "Loa not found")]
    [SwaggerResponse(400, "An error occurred")]
    public async Task<ActionResult<Response<Loa>>> DeleteLoa(int loaId)
    {
        try
        {
            return Ok(await _loaRepository.DeleteLoa(loaId, Request));
        }
        catch (LoaNotFoundException ex)
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
