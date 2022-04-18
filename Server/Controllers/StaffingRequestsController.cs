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
public class StaffingRequestsController : ControllerBase
{
    private readonly IStaffingRequestRepository _staffingRequestRepository;
    private readonly IValidator<StaffingRequest> _validator;
    private readonly IHub _sentryHub;

    public StaffingRequestsController(IStaffingRequestRepository staffingRequestRepository,
        IValidator<StaffingRequest> validator, IHub sentryHub)
    {
        _staffingRequestRepository = staffingRequestRepository;
        _validator = validator;
        _sentryHub = sentryHub;
    }

    #region Create

    [HttpPost]
    // todo auth
    [SwaggerResponse(201, "Created staffing request", typeof(Response<StaffingRequest>))]
    [SwaggerResponse(400, "An error occurred")]
    public async Task<ActionResult<Response<StaffingRequest>>> CreateStaffingRequest([FromBody] StaffingRequest staffingRequest)
    {
        try
        {
            var result = await _validator.ValidateAsync(staffingRequest);
            if (!result.IsValid)
            {
                return BadRequest(new Response<IList<ValidationFailure>>
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    Message = "Validation error",
                    Data = result.Errors
                });
            }
            return Ok(await _staffingRequestRepository.CreateStaffingRequest(staffingRequest, Request));
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
    [SwaggerResponse(200, "Got all staffing requests", typeof(Response<IList<StaffingRequest>>))]
    [SwaggerResponse(400, "An error occurred")]
    public async Task<ActionResult<Response<IList<StaffingRequest>>>> GetStaffingRequests(int skip = 0, int take = 10,
        StaffingRequestStatus status = StaffingRequestStatus.Pending)
    {
        try
        {
            return Ok(await _staffingRequestRepository.GetStaffingRequests(skip, take, status));
        }
        catch (Exception ex)
        {
            return _sentryHub.CaptureException(ex).ReturnActionResult();
        }
    }

    [HttpGet("{staffingRequestId:int}")]
    // todo auth
    [SwaggerResponse(200, "Got staffing request", typeof(Response<StaffingRequest>))]
    [SwaggerResponse(404, "Staffing request not found")]
    [SwaggerResponse(400, "An error occurred")]
    public async Task<ActionResult<Response<StaffingRequest>>> GetStaffingRequest(int staffingRequestId)
    {
        try
        {
            return Ok(await _staffingRequestRepository.GetStaffingRequest(staffingRequestId));
        }
        catch (StaffingRequestNotFoundException ex)
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
    [SwaggerResponse(200, "Updated staffing request", typeof(Response<StaffingRequest>))]
    [SwaggerResponse(400, "An error occurred")]
    public async Task<ActionResult<Response<StaffingRequest>>> UpdateStaffingRequest([FromBody] StaffingRequest staffingRequest)
    {
        try
        {
            var result = await _validator.ValidateAsync(staffingRequest);
            if (!result.IsValid)
            {
                return BadRequest(new Response<IList<ValidationFailure>>
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    Message = "Validation error",
                    Data = result.Errors
                });
            }
            return Ok(await _staffingRequestRepository.UpdateStaffingRequest(staffingRequest, Request));
        }
        catch (StaffingRequestNotFoundException ex)
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

    [HttpDelete("{staffingRequestId:int}")]
    // todo auth
    [SwaggerResponse(200, "Deleted staffing request", typeof(Response<StaffingRequest>))]
    [SwaggerResponse(404, "Staffing request not found")]
    [SwaggerResponse(400, "An error occurred")]
    public async Task<ActionResult<Response<StaffingRequest>>> DeleteStaffingRequest(int staffingRequestId)
    {
        try
        {
            return Ok(await _staffingRequestRepository.DeleteStaffingRequest(staffingRequestId, Request));
        }
        catch (StaffingRequestNotFoundException ex)
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
