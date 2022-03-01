using System.Net;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using ZDC.Server.Repositories.Interfaces;
using ZDC.Server.Services.Interfaces;
using ZDC.Shared;
using ZDC.Shared.Dtos;
using ZDC.Shared.Models;

namespace ZDC.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AirportsController : ControllerBase
{
    private readonly IAirportRepository _airportRepository;
    private readonly ILoggingService _loggingService;

    public AirportsController(IAirportRepository airportRepository, ILoggingService loggingService)
    {
        _airportRepository = airportRepository;
        _loggingService = loggingService;
    }

    [HttpPost]
    [SwaggerResponse(200, "Created airport", typeof(Response<Airport>))]
    [SwaggerResponse(400, "An error occurred")]
    public async Task<ActionResult<Response<Airport>>> CreateAirport([FromBody] Airport airport)
    {
        try
        {
            return Ok(await _airportRepository.CreateAirport(airport, Request));
        }
        catch (Exception ex)
        {
            return await _loggingService.AddDebugLog(Request, nameof(CreateAirport), ex.Message, ex.StackTrace);
        }
    }

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
            return await _loggingService.AddDebugLog(Request, nameof(GetAirports), ex.Message, ex.StackTrace);
        }
    }

    [HttpGet("{id:int}")]
    [SwaggerResponse(200, "Got airport", typeof(Response<Airport>))]
    [SwaggerResponse(404, "Airport not found")]
    [SwaggerResponse(400, "An error occurred")]
    public async Task<ActionResult<Response<Airport>>> GetAirport(int id)
    {
        try
        {
            return Ok(await _airportRepository.GetAirport(id));
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
            return await _loggingService.AddDebugLog(Request, nameof(GetAirport), ex.Message, ex.StackTrace);
        }
    }

    [HttpPut]
    [SwaggerResponse(200, "Updated airport", typeof(Response<Airport>))]
    [SwaggerResponse(404, "Airport not found")]
    [SwaggerResponse(400, "An error occurred")]
    public async Task<ActionResult<Response<Airport>>> UpdateAirport([FromBody] Airport airport)
    {
        try
        {
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
            return await _loggingService.AddDebugLog(Request, nameof(UpdateAirport), ex.Message, ex.StackTrace);
        }
    }

    [HttpDelete("{id:int}")]
    [SwaggerResponse(200, "Deleted airport", typeof(Response<Airport>))]
    [SwaggerResponse(404, "Airport not found")]
    [SwaggerResponse(400, "An error occurred")]
    public async Task<ActionResult<Response<Airport>>> DeleteAirport(int id)
    {
        try
        {
            return Ok(await _airportRepository.DeleteAirport(id, Request));
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
            return await _loggingService.AddDebugLog(Request, nameof(DeleteAirport), ex.Message, ex.StackTrace);
        }
    }
}