using ZDC.Shared.Dtos;
using ZDC.Shared.Models;

namespace ZDC.Server.Repositories.Interfaces;

public interface IVisitRequestRepository
{
    /// <summary>
    /// Add a new visit request
    /// </summary>
    /// <param name="visitRequest">Visit request data</param>
    /// <param name="request">Raw http request</param>
    /// <returns>Created visit request</returns>
    Task<Response<VisitRequest>> CreateVisitRequest(VisitRequest visitRequest, HttpRequest request);

    /// <summary>
    /// Get visit requests
    /// </summary>
    /// <param name="skip">Number to skip</param>
    /// <param name="take">Number to take</param>
    /// <param name="status">Status to get</param>
    /// <returns>Visit requests</returns>
    Task<ResponsePaging<IList<VisitRequest>>> GetVisitRequests(int skip, int take, VisitRequestStatus status);

    /// <summary>
    /// Get visit request by id
    /// </summary>
    /// <param name="visitRequestId">Visit request id to get</param>
    /// <exception cref="Shared.VisitRequestNotFoundException">Visit request not found</exception>
    /// <returns>Visit request if found</returns>
    Task<Response<VisitRequest>> GetVisitRequest(int visitRequestId);

    /// <summary>
    /// Update a visit request
    /// </summary>
    /// <param name="visitRequest">Updated visit request</param>
    /// <param name="request">Raw http request</param>
    /// <exception cref="Shared.VisitRequestNotFoundException">Visit request not found</exception>
    /// <returns>Updated visit request</returns>
    Task<Response<VisitRequest>> UpdateVisitRequest(VisitRequest visitRequest, HttpRequest request);

    /// <summary>
    /// Delete a visit request
    /// </summary>
    /// <param name="visitRequestId">Id of request to delete</param>
    /// <param name="request">Raw http request</param>
    /// <exception cref="Shared.VisitRequestNotFoundException">Visit request not found</exception>
    /// <returns>Deleted visit request</returns>
    Task<Response<VisitRequest>> DeleteVisitRequest(int visitRequestId, HttpRequest request);
}
