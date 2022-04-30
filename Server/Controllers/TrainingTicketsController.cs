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
public class TrainingTicketsController : ControllerBase
{
    private readonly ITrainingTicketRepository _trainingTicketRepository;
    private readonly IValidator<TrainingTicket> _validator;
    private readonly IHub _sentryHub;

    public TrainingTicketsController(ITrainingTicketRepository trainingTicketRepository, IValidator<TrainingTicket> validator, IHub sentryHub)
    {
        _trainingTicketRepository = trainingTicketRepository;
        _validator = validator;
        _sentryHub = sentryHub;
    }

    #region Create

    [HttpPost]
    // todo auth
    [SwaggerResponse(201, "Created training ticket", typeof(Response<TrainingTicket>))]
    [SwaggerResponse(400, "An error occurred")]
    public async Task<ActionResult<TrainingTicket>> CreateTrainingTicket([FromBody] TrainingTicket trainingTicket)
    {
        try
        {
            var result = await _validator.ValidateAsync(trainingTicket);
            if (!result.IsValid)
            {
                return BadRequest(new Response<IList<ValidationFailure>>
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    Message = "Validation error",
                    Data = result.Errors
                });
            }
            return Ok(await _trainingTicketRepository.CreateTrainingTicket(trainingTicket, Request));
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
    [SwaggerResponse(200, "Got training tickets", typeof(ResponsePaging<IList<TrainingTicket>>))]
    [SwaggerResponse(404, "User not found")]
    [SwaggerResponse(400, "An error occurred")]
    public async Task<ActionResult<IList<TrainingTicket>>> GetTrainingTickets(int skip = 0, int take = 10)
    {
        try
        {
            return Ok(await _trainingTicketRepository.GetTrainingTickets(skip, take));
        }
        catch (Exception ex)
        {
            return _sentryHub.CaptureException(ex).ReturnActionResult();
        }
    }

    [HttpGet("user/{userId:int}")]
    // todo auth
    [SwaggerResponse(200, "Got user training tickets", typeof(Response<IList<TrainingTicket>>))]
    [SwaggerResponse(400, "An error occurred")]
    public async Task<ActionResult<IList<TrainingTicket>>> GetUserTrainingTickets(int userId, int skip = 0, int take = 10)
    {
        try
        {
            return Ok(await _trainingTicketRepository.GetUserTrainingTickets(userId, skip, take));
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

    [HttpGet("user")]
    // todo auth
    [SwaggerResponse(200, "Got user training tickets", typeof(Response<IList<TrainingTicket>>))]
    [SwaggerResponse(404, "User not found")]
    [SwaggerResponse(400, "An error occurred")]
    public async Task<ActionResult<IList<TrainingTicket>>> GetUserTrainingTickets(int skip = 0, int take = 10)
    {
        try
        {
            return Ok(await _trainingTicketRepository.GetUserTrainingTickets(skip, take, Request));
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

    [HttpGet("{trainingTicketId:int}")]
    // todo auth
    [SwaggerResponse(200, "Got training ticket", typeof(Response<TrainingTicket>))]
    [SwaggerResponse(404, "User not found")]
    [SwaggerResponse(400, "An error occurred")]
    public async Task<ActionResult<IList<TrainingTicket>>> GetTrainingTicket(int trainingTicketId)
    {
        try
        {
            return Ok(await _trainingTicketRepository.GetTrainingTicket(trainingTicketId));
        }
        catch (TrainingTicketNotFoundException ex)
        {
            return NotFound(new Response<string>
            {
                StatusCode = HttpStatusCode.NotFound,
                Message = ex.Message,
                Data = string.Empty
            });
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

    #region Update

    [HttpPut]
    // todo auth
    [SwaggerResponse(200, "Updated training ticket", typeof(Response<TrainingTicket>))]
    [SwaggerResponse(404, "Training ticket or user not found")]
    [SwaggerResponse(400, "An error occurred")]
    public async Task<ActionResult<Response<TrainingTicket>>> UpdateTrainingTicket([FromBody] TrainingTicket trainingTicket)
    {
        try
        {
            var result = await _validator.ValidateAsync(trainingTicket);
            if (!result.IsValid)
            {
                return BadRequest(new Response<IList<ValidationFailure>>
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    Message = "Validation error",
                    Data = result.Errors
                });
            }
            return Ok(await _trainingTicketRepository.UpdateTrainingTicket(trainingTicket, Request));
        }
        catch (TrainingTicketNotFoundException ex)
        {
            return NotFound(new Response<string>
            {
                StatusCode = HttpStatusCode.NotFound,
                Message = ex.Message,
                Data = string.Empty
            });
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

    #region Delete

    [HttpDelete("{trainingTicketId:int}")]
    // todo auth
    [SwaggerResponse(200, "Deleted training ticket", typeof(Response<TrainingTicket>))]
    [SwaggerResponse(404, "Training ticket not found")]
    [SwaggerResponse(400, "An error occurred")]
    public async Task<ActionResult<Response<TrainingTicket>>> DeleteTrainingTicket(int trainingTicketId)
    {
        try
        {
            return Ok(await _trainingTicketRepository.DeleteTrainingTicket(trainingTicketId, Request));
        }
        catch (TrainingTicketNotFoundException ex)
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
