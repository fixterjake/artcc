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
public class AnnouncementsController : ControllerBase
{
    private readonly IAnnouncementRepository _announcementRepository;
    private readonly IValidator<Announcement> _validator;
    private readonly IHub _sentryHub;

    public AnnouncementsController(IAnnouncementRepository announcementRepository, IValidator<Announcement> validator, IHub sentryHub)
    {
        _announcementRepository = announcementRepository;
        _validator = validator;
        _sentryHub = sentryHub;
    }

    #region Create

    [HttpPost]
    // todo auth
    [SwaggerResponse(200, "Created announcement", typeof(Response<Announcement>))]
    [SwaggerResponse(404, "User not found")]
    [SwaggerResponse(400, "An error occurred")]
    public async Task<ActionResult<Response<Announcement>>> CreateAnnouncement([FromBody] Announcement announcement)
    {
        try
        {
            var result = await _validator.ValidateAsync(announcement);
            if (!result.IsValid)
            {
                return BadRequest(new Response<IList<ValidationFailure>>
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    Message = "Validation error",
                    Data = result.Errors
                });
            }
            return Ok(await _announcementRepository.CreateAnnouncement(announcement, Request));
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
    [SwaggerResponse(200, "Got all announcements", typeof(Response<IList<Announcement>>))]
    [SwaggerResponse(400, "An error occurred")]
    public async Task<ActionResult<Response<IList<Announcement>>>> GetAnnouncements(int skip = 0, int take = 10)
    {
        try
        {
            return Ok(await _announcementRepository.GetAnnouncements(skip, take));
        }
        catch (Exception ex)
        {
            return _sentryHub.CaptureException(ex).ReturnActionResult();
        }
    }

    [HttpGet("{announcementId:int}")]
    [SwaggerResponse(200, "Got announcements", typeof(Response<Announcement>))]
    [SwaggerResponse(400, "Announcement not found")]
    [SwaggerResponse(400, "An error occurred")]
    public async Task<ActionResult<Response<Announcement>>> GetAnnouncement(int announcementId)
    {
        try
        {
            return Ok(await _announcementRepository.GetAnnouncement(announcementId));
        }
        catch (AnnouncementNotFoundException ex)
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
    [SwaggerResponse(200, "Updated announcement", typeof(Response<Announcement>))]
    [SwaggerResponse(404, "User or announcement not found")]
    [SwaggerResponse(400, "An error occurred")]
    public async Task<ActionResult<Response<Announcement>>> UpdateAnnouncement([FromBody] Announcement announcement)
    {
        try
        {
            var result = await _validator.ValidateAsync(announcement);
            if (!result.IsValid)
            {
                return BadRequest(new Response<IList<ValidationFailure>>
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    Message = "Validation error",
                    Data = result.Errors
                });
            }
            return Ok(await _announcementRepository.UpdateAnnouncement(announcement, Request));
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
        catch (AnnouncementNotFoundException ex)
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

    [HttpDelete("{AnnouncementId:int}")]
    // todo auth
    [SwaggerResponse(200, "Deleted announcement", typeof(Response<Announcement>))]
    [SwaggerResponse(404, "News not found")]
    [SwaggerResponse(400, "An error occurred")]
    public async Task<ActionResult<Response<Announcement>>> DeleteAnnouncement(int announcementId)
    {
        try
        {
            return Ok(await _announcementRepository.DeleteAnnouncement(announcementId, Request));
        }
        catch (AnnouncementNotFoundException ex)
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
