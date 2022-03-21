using System.Net;
using FluentValidation;
using FluentValidation.Results;
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
public class CommentsController : ControllerBase
{
    private readonly ICommentRepository _commentRepository;
    private readonly ILoggingService _loggingService;
    private readonly IValidator<Comment> _validator;

    public CommentsController(ICommentRepository commentRepository, ILoggingService loggingService, IValidator<Comment> validator)
    {
        _commentRepository = commentRepository;
        _loggingService = loggingService;
        _validator = validator;
    }

    [HttpPost]
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
            return await _loggingService.AddDebugLog(Request, nameof(CreateComment), ex.Message, ex.StackTrace ?? "N/A");
        }
    }

    [HttpGet("{id:int}")]
    [SwaggerResponse(200, "Got user comments", typeof(Response<IList<Comment>>))]
    [SwaggerResponse(404, "User not found")]
    [SwaggerResponse(400, "An error occurred")]
    public async Task<ActionResult<Response<IList<Comment>>>> GetUserComments(int id)
    {
        try
        {
            return Ok(await _commentRepository.GetUserComments(id));
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
            return await _loggingService.AddDebugLog(Request, nameof(GetUserComments), ex.Message, ex.StackTrace ?? "N/A");
        }
    }

    [HttpPut]
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
            return await _loggingService.AddDebugLog(Request, nameof(UpdateComment), ex.Message, ex.StackTrace ?? "N/A");
        }
    }

    [HttpDelete("{id:int}")]
    [SwaggerResponse(200, "Deleted comment", typeof(Response<Comment>))]
    [SwaggerResponse(404, "Comment not found")]
    [SwaggerResponse(400, "An error occurred")]
    public async Task<ActionResult<Response<Comment>>> DeleteComment(int id)
    {
        try
        {
            return Ok(await _commentRepository.DeleteComment(id, Request));
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
            return await _loggingService.AddDebugLog(Request, nameof(DeleteComment), ex.Message, ex.StackTrace ?? "N/A");
        }
    }
}