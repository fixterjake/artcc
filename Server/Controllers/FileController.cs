using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Sentry;
using Swashbuckle.AspNetCore.Annotations;
using System.Net;
using ZDC.Server.Extensions;
using ZDC.Server.Repositories.Interfaces;
using ZDC.Server.Services.Interfaces;
using ZDC.Shared.Dtos;
using File = ZDC.Shared.Models.File;

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

    [HttpPost]
    // todo auth
    [SwaggerResponse(200, "Created file", typeof(Response<File>))]
    [SwaggerResponse(400, "An error occurred")]
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
}
