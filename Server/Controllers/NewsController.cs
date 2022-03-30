using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Sentry;
using Swashbuckle.AspNetCore.Annotations;
using System.Net;
using ZDC.Server.Extensions;
using ZDC.Server.Repositories;
using ZDC.Server.Repositories.Interfaces;
using ZDC.Shared;
using ZDC.Shared.Dtos;
using ZDC.Shared.Models;

namespace ZDC.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class NewsController : ControllerBase
{
    private readonly INewsRepository _newsRepository;
    private readonly IValidator<News> _validator;
    private readonly IHub _sentryHub;

    public NewsController(INewsRepository newsRepository, IValidator<News> validator, IHub sentryHub)
    {
        _newsRepository = newsRepository;
        _validator = validator;
        _sentryHub = sentryHub;
    }

    #region Create

    [HttpPost]
    // todo auth
    [SwaggerResponse(200, "Created news", typeof(Response<News>))]
    [SwaggerResponse(404, "User not found")]
    [SwaggerResponse(400, "An error occurred")]
    public async Task<ActionResult<Response<News>>> CreateNews([FromBody] News news)
    {
        try
        {
            var result = await _validator.ValidateAsync(news);
            if (!result.IsValid)
            {
                return BadRequest(new Response<IList<ValidationFailure>>
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    Message = "Validation error",
                    Data = result.Errors
                });
            }
            return Ok(await _newsRepository.CreateNews(news, Request));
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
    [SwaggerResponse(200, "Got all news", typeof(Response<IList<News>>))]
    [SwaggerResponse(400, "An error occurred")]
    public async Task<ActionResult<Response<IList<News>>>> GetNews()
    {
        try
        {
            return Ok(await _newsRepository.GetNews());
        }
        catch (Exception ex)
        {
            return _sentryHub.CaptureException(ex).ReturnActionResult();
        }
    }

    [HttpGet("{newsId:int}")]
    [SwaggerResponse(200, "Got news", typeof(Response<News>))]
    [SwaggerResponse(400, "News not found")]
    [SwaggerResponse(400, "An error occurred")]
    public async Task<ActionResult<Response<News>>> GetNews(int newsId)
    {
        try
        {
            return Ok(await _newsRepository.GetNews(newsId));
        }
        catch (NewsNotFoundException ex)
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
    [SwaggerResponse(200, "Updated news", typeof(Response<News>))]
    [SwaggerResponse(404, "User or news not found")]
    [SwaggerResponse(400, "An error occurred")]
    public async Task<ActionResult<Response<News>>> UpdateNews([FromBody] News news)
    {
        try
        {
            var result = await _validator.ValidateAsync(news);
            if (!result.IsValid)
            {
                return BadRequest(new Response<IList<ValidationFailure>>
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    Message = "Validation error",
                    Data = result.Errors
                });
            }
            return Ok(await _newsRepository.UpdateNews(news, Request));
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
        catch (NewsNotFoundException ex)
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

    [HttpDelete("{newsId:int}")]
    // todo auth
    [SwaggerResponse(200, "Deleted news", typeof(Response<News>))]
    [SwaggerResponse(404, "News not found")]
    [SwaggerResponse(400, "An error occurred")]
    public async Task<ActionResult<Response<News>>> DeleteNews(int newsId)
    {
        try
        {
            return Ok(await _newsRepository.DeleteNews(newsId, Request));
        }
        catch (NewsNotFoundException ex)
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
