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
public class CommentsController : ControllerBase
{
    private readonly ICommentRepository _commentRepository;
    private readonly IValidator<Comment> _validator;
    private readonly IHub _sentryHub;

    public CommentsController(ICommentRepository commentRepository, IValidator<Comment> validator, IHub sentryHub)
    {
        _commentRepository = commentRepository;
        _validator = validator;
        _sentryHub = sentryHub;
    }

    #region Create

    [HttpPost]
    // todo auth
    [SwaggerResponse(200, "Created comment", typeof(Response<Comment>))]
    [SwaggerResponse(400, "An error occurred")]
    public async Task<ActionResult<Response<Comment>>> CreateComment([FromBody] Comment comment)
    {
        try
        {
            var result = await _validator.ValidateAsync(comment);
            if (!result.IsValid)
            {
                return BadRequest(new Response<IList<ValidationFailure>>
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    Message = "Validation error",
                    Data = result.Errors
                });
            }
            return Ok(await _commentRepository.CreateComment(comment, Request));
        }
        catch (Exception ex)
        {
            return _sentryHub.CaptureException(ex).ReturnActionResult();
        }
    }

    #endregion

    #region Read

    [HttpGet("{userId:int}")]
    // todo auth
    [SwaggerResponse(200, "Got user comments", typeof(ResponsePaging<IList<Comment>>))]
    [SwaggerResponse(404, "User not found")]
    [SwaggerResponse(400, "An error occurred")]
    public async Task<ActionResult<ResponsePaging<IList<Comment>>>> GetUserComments(int userId, int skip = 0, int take = 10)
    {
        try
        {
            return Ok(await _commentRepository.GetUserComments(userId, skip, take));
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
    // todo auth
    [SwaggerResponse(200, "Updated comment", typeof(Response<Comment>))]
    [SwaggerResponse(404, "Comment or user not found")]
    [SwaggerResponse(400, "An error occurred")]
    public async Task<ActionResult<Response<Airport>>> UpdateComment([FromBody] Comment comment)
    {
        try
        {
            var result = await _validator.ValidateAsync(comment);
            if (!result.IsValid)
            {
                return BadRequest(new Response<IList<ValidationFailure>>
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    Message = "Validation error",
                    Data = result.Errors
                });
            }
            return Ok(await _commentRepository.UpdateComment(comment, Request));
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

    [HttpDelete("{commentId:int}")]
    // todo auth
    [SwaggerResponse(200, "Deleted comment", typeof(Response<Comment>))]
    [SwaggerResponse(404, "Comment not found")]
    [SwaggerResponse(400, "An error occurred")]
    public async Task<ActionResult<Response<Comment>>> DeleteComment(int commentId)
    {
        try
        {
            return Ok(await _commentRepository.DeleteComment(commentId, Request));
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