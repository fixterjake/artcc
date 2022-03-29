using System.Net;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Sentry;
using Swashbuckle.AspNetCore.Annotations;
using ZDC.Server.Extensions;
using ZDC.Server.Repositories;
using ZDC.Server.Repositories.Interfaces;
using ZDC.Server.Services.Interfaces;
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
    [SwaggerResponse(200, "Got loas", typeof(Response<IList<Loa>>))]
    [SwaggerResponse(400, "An error occurred")]
    public async Task<ActionResult<Response<IList<Loa>>>> GetLoas()
    {
        try
        {
            return Ok(await _loaRepository.GetLoas());
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
        catch (Exception ex)
        {
            return _sentryHub.CaptureException(ex).ReturnActionResult();
        }
    }

    #endregion
}
