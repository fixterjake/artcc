using Amazon.Auth.AccessControlPolicy;
using FluentValidation;
using FluentValidation.Results;
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
    [SwaggerResponse(201, "Created airport", typeof(Response<Airport>))]
    [SwaggerResponse(400, "An error occurred")]
    [SwaggerResponse(401, "Unauthorized")]
    public async Task<ActionResult<Response<Airport>>> CreateAirport([FromBody] Airport airport)
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
            return Ok(await _airportRepository.CreateAirport(airport, Request));
        }
        catch (Exception ex)
        {
            return _sentryHub.CaptureException(ex).ReturnActionResult();
        }
    }

    #endregion

    #region Read

    [HttpGet]
    [SwaggerResponse(200, "Got all airports", typeof(Response<IList<Airport>>))]
    [SwaggerResponse(400, "An error occurred")]
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
    [SwaggerResponse(200, "Got airport", typeof(Response<Airport>))]
    [SwaggerResponse(404, "Airport not found")]
    [SwaggerResponse(400, "An error occurred")]
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
    [SwaggerResponse(200, "Updated airport", typeof(Response<Airport>))]
    [SwaggerResponse(404, "Airport not found")]
    [SwaggerResponse(400, "An error occurred")]
    [SwaggerResponse(401, "Unauthorized")]
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
    [SwaggerResponse(200, "Deleted airport", typeof(Response<Airport>))]
    [SwaggerResponse(404, "Airport not found")]
    [SwaggerResponse(400, "An error occurred")]
    [SwaggerResponse(401, "Unauthorized")]
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