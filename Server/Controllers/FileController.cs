using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sentry;
using Swashbuckle.AspNetCore.Annotations;
using System.Net;
using ZDC.Server.Extensions;
using ZDC.Server.Repositories.Interfaces;
using ZDC.Shared.Dtos;
using File = ZDC.Shared.Models.File;
using FileNotFoundException = ZDC.Shared.FileNotFoundException;

namespace ZDC.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FileController : ControllerBase
{
    private readonly IFileRepository _fileRepository;
    private readonly IValidator<File> _validator;
    private readonly IHub _sentryHub;

    public FileController(IFileRepository fileRepository, IValidator<File> validator, IHub sentryHub)
    {
        _fileRepository = fileRepository;
        _validator = validator;
        _sentryHub = sentryHub;
    }

    #region Create

    [HttpPost]
    [Authorize(Policy = "CanFiles")]
    [SwaggerResponse(200, "Created file", typeof(Response<File>))]
    [SwaggerResponse(400, "An error occurred")]
    [SwaggerResponse(401, "Unauthorized")]
    public async Task<ActionResult<Response<File>>> CreateFile([FromBody] File file)
    {
        try
        {
            var result = _validator.Validate(file);
            if (!result.IsValid)
            {
                return BadRequest(new Response<IList<ValidationFailure>>
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    Message = "Validation error",
                    Data = result.Errors
                });
            }
            return Ok(await _fileRepository.CreateFile(file, Request));
        }
        catch (Exception ex)
        {
            return _sentryHub.CaptureException(ex).ReturnActionResult();
        }
    }

    #endregion

    #region Read

    [HttpGet]
    [SwaggerResponse(200, "Got all files", typeof(Response<IList<File>>))]
    [SwaggerResponse(400, "An error occurred")]
    public async Task<ActionResult<Response<IList<File>>>> GetFiles()
    {
        try
        {
            return Ok(await _fileRepository.GetFiles(Request));
        }
        catch (Exception ex)
        {
            return _sentryHub.CaptureException(ex).ReturnActionResult();
        }
    }

    [HttpGet("{fileId:int}")]
    [SwaggerResponse(200, "Got file", typeof(Response<File>))]
    [SwaggerResponse(404, "File not found")]
    [SwaggerResponse(400, "An error occurred")]
    public async Task<ActionResult<Response<File>>> GetFile(int fileId)
    {
        try
        {
            return Ok(await _fileRepository.GetFile(fileId, Request));
        }
        catch (FileNotFoundException ex)
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
    [Authorize(Policy = "CanFiles")]
    [SwaggerResponse(200, "Updated file", typeof(Response<File>))]
    [SwaggerResponse(404, "File not found")]
    [SwaggerResponse(400, "An error occurred")]
    [SwaggerResponse(401, "Unauthorized")]
    public async Task<ActionResult<Response<File>>> UpdateFile([FromBody] File file)
    {
        try
        {
            var result = _validator.Validate(file);
            if (!result.IsValid)
            {
                return BadRequest(new Response<IList<ValidationFailure>>
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    Message = "Validation error",
                    Data = result.Errors
                });
            }
            return Ok(await _fileRepository.UpdateFile(file, Request));
        }
        catch (FileNotFoundException ex)
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

    [HttpDelete("{fileId:int}")]
    [Authorize(Policy = "CanFiles")]
    [SwaggerResponse(200, "Deleted file", typeof(Response<File>))]
    [SwaggerResponse(404, "File not found")]
    [SwaggerResponse(400, "An error occurred")]
    [SwaggerResponse(401, "Unauthorized")]
    public async Task<ActionResult<Response<File>>> DeleteFile(int fileId)
    {
        try
        {
            return Ok(await _fileRepository.DeleteFile(fileId, Request));
        }
        catch (FileNotFoundException ex)
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
