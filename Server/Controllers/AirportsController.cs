using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sentry;
using System.Net;
using ZDC.Server.Extensions;
using ZDC.Server.Repositories.Interfaces;
using ZDC.Shared;
using ZDC.Shared.Dtos;
using ZDC.Shared.Models;

namespace ZDC.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AirportsController : ControllerBase
{
    private readonly IAirportRepository _airportRepository;
    private readonly IValidator<Airport> _validator;
    private readonly IHub _sentryHub;

    public AirportsController(IAirportRepository airportRepository, IValidator<Airport> validator, IHub sentryHub)
    {
        _airportRepository = airportRepository;
        _validator = validator;
        _sentryHub = sentryHub;
    }

    #region Create

    [HttpPost]
    [Authorize(Policy = "CanAirports")]
    [ProducesResponseType(typeof(Response<Airport>), 201)]
    [ProducesResponseType(typeof(Response<string>), 401)]
    [ProducesResponseType(typeof(Response<string>), 400)]
    [ProducesResponseType(typeof(Response<string>), 500)]
    public async Task<ActionResult<Response<Airport>>> CreateAirport([FromBody] Airport airport)
    {
        try
        {
            var valid = await _validator.ValidateAsync(airport);
            if (!valid.IsValid)
            {
                return BadRequest(new Response<IList<ValidationFailure>>
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    Message = "Validation error",
                    Data = valid.Errors
                });
            }
            var result = await _airportRepository.CreateAirport(airport, Request);
            return CreatedAtAction(nameof(GetAirport), new { airportId = result?.Data?.Id }, result);
        }
        catch (Exception ex)
        {
            return _sentryHub.CaptureException(ex).ReturnActionResult();
        }
    }

    #endregion

    #region Read

    [HttpGet]
    [ProducesResponseType(typeof(Response<IList<Airport>>), 200)]
    [ProducesResponseType(typeof(Response<string>), 500)]
    public async Task<ActionResult<Response<IList<Airport>>>> GetAirports()
    {
        try
        {
            return Ok(await _airportRepository.GetAirports());
        }
        catch (Exception ex)
        {
            return _sentryHub.CaptureException(ex).ReturnActionResult();
        }
    }

    [HttpGet("{airportId:int}")]
    [ProducesResponseType(typeof(Response<IList<Airport>>), 200)]
    [ProducesResponseType(typeof(Response<string>), 404)]
    [ProducesResponseType(typeof(Response<string>), 500)]
    public async Task<ActionResult<Response<Airport>>> GetAirport(int airportId)
    {
        try
        {
            return Ok(await _airportRepository.GetAirport(airportId));
        }
        catch (AirportNotFoundException ex)
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
    [Authorize(Policy = "CanAirports")]
    [ProducesResponseType(typeof(Response<Airport>), 200)]
    [ProducesResponseType(typeof(Response<string>), 401)]
    [ProducesResponseType(typeof(Response<string>), 400)]
    [ProducesResponseType(typeof(Response<string>), 500)]
    public async Task<ActionResult<Response<Airport>>> UpdateAirport([FromBody] Airport airport)
    {
        try
        {
            var result = await _validator.ValidateAsync(airport);
            if (!result.IsValid)
            {
                return BadRequest(new Response<IList<ValidationFailure>>
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    Message = "Validation error",
                    Data = result.Errors
                });
            }
            return Ok(await _airportRepository.UpdateAirport(airport, Request));
        }
        catch (AirportNotFoundException ex)
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

    [HttpDelete("{airportId:int}")]
    [Authorize(Policy = "CanAirports")]
    [ProducesResponseType(typeof(Response<Airport>), 200)]
    [ProducesResponseType(typeof(Response<string>), 401)]
    [ProducesResponseType(typeof(Response<string>), 400)]
    [ProducesResponseType(typeof(Response<string>), 500)]
    public async Task<ActionResult<Response<Airport>>> DeleteAirport(int airportId)
    {
        try
        {
            return Ok(await _airportRepository.DeleteAirport(airportId, Request));
        }
        catch (AirportNotFoundException ex)
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