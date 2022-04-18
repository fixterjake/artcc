using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Sentry;
using Swashbuckle.AspNetCore.Annotations;
using System.Net;
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
public class FeedbackController : ControllerBase
{
    private readonly IFeedbackRepository _feedbackRepository;
    private readonly IValidator<Feedback> _validator;
    private readonly IHub _sentryHub;

    public FeedbackController(IFeedbackRepository feedbackRepository, IValidator<Feedback> validator, IHub sentryHub)
    {
        _feedbackRepository = feedbackRepository;
        _validator = validator;
        _sentryHub = sentryHub;
    }

    #region Create

    [HttpPost]
    // todo auth
    [SwaggerResponse(200, "Created feedback", typeof(Response<string>))]
    [SwaggerResponse(404, "User not found")]
    [SwaggerResponse(400, "An error occurred")]
    public async Task<ActionResult<Response<string>>> CreateFeedback([FromBody] Feedback feedback)
    {
        try
        {
            var result = await _validator.ValidateAsync(feedback);
            if (!result.IsValid)
            {
                return BadRequest(new Response<IList<ValidationFailure>>
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    Message = "Validation error",
                    Data = result.Errors
                });
            }
            return Ok(await _feedbackRepository.CreateFeedback(feedback, Request));
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
    [SwaggerResponse(200, "Got all feedback", typeof(Response<IList<Feedback>>))]
    [SwaggerResponse(400, "An error occurred")]
    public async Task<ActionResult<Response<IList<Feedback>>>> GetFeedback(int skip = 0, int take = 10)
    {
        try
        {
            return Ok(await _feedbackRepository.GetFeedback(skip, take));
        }
        catch (Exception ex)
        {
            return _sentryHub.CaptureException(ex).ReturnActionResult();
        }
    }


    [HttpGet("{feedbackId:int}")]
    // todo auth
    [SwaggerResponse(200, "Got feedback", typeof(Response<Feedback>))]
    [SwaggerResponse(404, "Feedback not found")]
    [SwaggerResponse(400, "An error occurred")]
    public async Task<ActionResult<Response<IList<Feedback>>>> GetFeedback(int feedbackId)
    {
        try
        {
            return Ok(await _feedbackRepository.GetFeedback(feedbackId));
        }
        catch (FeedbackNotFoundException ex)
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
    [SwaggerResponse(200, "Updated feedback", typeof(Response<Feedback>))]
    [SwaggerResponse(404, "Feedback or user not found")]
    [SwaggerResponse(400, "An error occurred")]
    public async Task<ActionResult<Response<Feedback>>> UpdateFeedback([FromBody] Feedback feedback)
    {
        try
        {
            var result = await _validator.ValidateAsync(feedback);
            if (!result.IsValid)
            {
                return BadRequest(new Response<IList<ValidationFailure>>
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    Message = "Validation error",
                    Data = result.Errors
                });
            }
            return Ok(await _feedbackRepository.UpdateFeedback(feedback, Request));
        }
        catch (FeedbackNotFoundException ex)
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

    [HttpDelete("{feedbackId:int}")]
    // todo auth
    [SwaggerResponse(200, "Deleted feedback", typeof(Response<Feedback>))]
    [SwaggerResponse(404, "Feedback not found")]
    [SwaggerResponse(400, "An error occurred")]
    public async Task<ActionResult<Response<Feedback>>> DeleteFeedback(int feedbackId)
    {
        try
        {
            return Ok(await _feedbackRepository.DeleteFeedback(feedbackId, Request));
        }
        catch (FeedbackNotFoundException ex)
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
