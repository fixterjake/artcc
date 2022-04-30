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
public class PositionsController : ControllerBase
{
    public readonly IPositionRepository _positionRepository;
    private readonly IHub _sentryHub;

    public PositionsController(IPositionRepository positionRepository, IHub sentryHub)
    {
        _positionRepository = positionRepository;
        _sentryHub = sentryHub;
    }

    #region Create

    [HttpPost]
    // todo auth
    [SwaggerResponse(201, "Created position", typeof(Response<Position>))]
    [SwaggerResponse(400, "An error occurred")]
    public async Task<ActionResult<Response<Position>>> CreatePosition([FromBody] Position position)
    {
        try
        {
            return Ok(await _positionRepository.CreatePosition(position, Request));
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
    [SwaggerResponse(200, "Got positions", typeof(Response<IList<Position>>))]
    [SwaggerResponse(400, "An error occurred")]
    public async Task<ActionResult<Response<IList<Position>>>> GetPositions()
    {
        try
        {
            return Ok(await _positionRepository.GetPositions());
        }
        catch (Exception ex)
        {
            return _sentryHub.CaptureException(ex).ReturnActionResult();
        }
    }

    #endregion

    #region Delete

    [HttpDelete("{positionId:int}")]
    // todo auth
    [SwaggerResponse(200, "Deleted position", typeof(Response<Position>))]
    [SwaggerResponse(400, "An error occurred")]
    public async Task<ActionResult<Response<IList<Position>>>> DeletePosition(int positionId)
    {
        try
        {
            return Ok(await _positionRepository.DeletePosition(positionId, Request));
        }
        catch (PositionNotFoundException ex)
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
