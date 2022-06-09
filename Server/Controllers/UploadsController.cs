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
public class UploadsController : ControllerBase
{
    private readonly IUploadRepository _uploadRepository;
    private readonly IHub _sentryHub;

    public UploadsController(IUploadRepository uploadRepository, IHub sentryHub)
    {
        _uploadRepository = uploadRepository;
        _sentryHub = sentryHub;
    }

    [HttpPost]
    [Authorize(Policy = "IsStaff")]
    [SwaggerResponse(200, "Created upload", typeof(Response<Upload>))]
    [SwaggerResponse(400, "An error occurred")]
    [SwaggerResponse(401, "Unauthorized")]
    public async Task<ActionResult<Response<Upload>>> CreateUpload(string type)
    {
        try
        {
            return Ok(await _uploadRepository.AddUpload(type, Request));
        }
        catch (UploadNotFoundException ex)
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
}
