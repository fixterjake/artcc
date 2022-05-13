using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Authorization;
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
public class VisitRequestsController : ControllerBase
{
    private readonly IVisitRequestRepository _visitRequestRepository;
    private readonly IValidator<VisitRequest> _validator;
    private readonly IHub _sentryHub;

    public VisitRequestsController(IVisitRequestRepository visitRequestRepository, IValidator<VisitRequest> validator, IHub sentryHub)
    {
        _visitRequestRepository = visitRequestRepository;
        _validator = validator;
        _sentryHub = sentryHub;
    }

    #region Create

    [HttpPost]
    [Authorize]
    [SwaggerResponse(201, "Created visit request", typeof(Response<VisitRequest>))]
    [SwaggerResponse(400, "An error occurred")]
    [SwaggerResponse(401, "Unauthorized")]
    public async Task<ActionResult<Response<VisitRequest>>> CreateVisitRequest(VisitRequest visitRequest)
    {
        try
        {
            var result = await _validator.ValidateAsync(visitRequest);
            if (!result.IsValid)
            {
                return BadRequest(new Response<IList<ValidationFailure>>
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    Message = "Validation error",
                    Data = result.Errors
                });
            }
            return Ok(await _visitRequestRepository.CreateVisitRequest(visitRequest, Request));
        }
        catch (Exception ex)
        {
            return _sentryHub.CaptureException(ex).ReturnActionResult();
        }
    }

    #endregion

    #region Read

    [HttpGet]
    [Authorize(Policy = "IsSeniorStaff")]
    [SwaggerResponse(200, "Got visit requests", typeof(ResponsePaging<IList<VisitRequest>>))]
    [SwaggerResponse(400, "An error occurred")]
    [SwaggerResponse(401, "Unauthorized")]
    public async Task<ActionResult<ResponsePaging<IList<VisitRequest>>>> GetVisitRequests(int skip = 0, int take = 20,
        VisitRequestStatus status = VisitRequestStatus.Pending)
    {
        try
        {
            return Ok(await _visitRequestRepository.GetVisitRequests(skip, take, status));
        }
        catch (Exception ex)
        {
            return _sentryHub.CaptureException(ex).ReturnActionResult();
        }
    }

    [HttpGet("{visitRequestId:int}")]
    [Authorize(Policy = "IsSeniorStaff")]
    [SwaggerResponse(200, "Got visit requests", typeof(ResponsePaging<IList<VisitRequest>>))]
    [SwaggerResponse(404, "Visit request not found")]
    [SwaggerResponse(400, "An error occurred")]
    [SwaggerResponse(401, "Unauthorized")]
    public async Task<ActionResult<Response<VisitRequest>>> GetVisitRequest(int visitRequestId)
    {
        try
        {
            return Ok(await _visitRequestRepository.GetVisitRequest(visitRequestId));
        }
        catch (VisitRequestNotFoundException ex)
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
    [Authorize(Policy = "IsSeniorStaff")]
    [SwaggerResponse(200, "Updated visit request", typeof(Response<VisitRequest>))]
    [SwaggerResponse(404, "Visit request not found")]
    [SwaggerResponse(400, "An error occurred")]
    [SwaggerResponse(401, "Unauthorized")]
    public async Task<ActionResult<Response<VisitRequest>>> UpdateVisitRequest(VisitRequest visitRequest)
    {
        try
        {
            var result = await _validator.ValidateAsync(visitRequest);
            if (!result.IsValid)
            {
                return BadRequest(new Response<IList<ValidationFailure>>
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    Message = "Validation error",
                    Data = result.Errors
                });
            }
            return Ok(await _visitRequestRepository.UpdateVisitRequest(visitRequest, Request));
        }
        catch (VisitRequestNotFoundException ex)
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

    [HttpDelete("{visitRequestId:int}")]
    [Authorize(Policy = "IsSeniorStaff")]
    [SwaggerResponse(200, "Deleted visit request", typeof(Response<VisitRequest>))]
    [SwaggerResponse(404, "Visit request not found")]
    [SwaggerResponse(400, "An error occurred")]
    [SwaggerResponse(401, "Unauthorized")]
    public async Task<ActionResult<Response<VisitRequest>>> DeleteVisitRequest(int visitRequestId)
    {
        try
        {
            return Ok(await _visitRequestRepository.DeleteVisitRequest(visitRequestId, Request));
        }
        catch (VisitRequestNotFoundException ex)
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
