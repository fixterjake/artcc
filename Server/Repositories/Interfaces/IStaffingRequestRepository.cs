using ZDC.Shared.Dtos;
using ZDC.Shared.Models;

namespace ZDC.Server.Repositories.Interfaces;

public interface IStaffingRequestRepository
{
    /// <summary>
    /// Create staffing request
    /// </summary>
    /// <param name="staffingRequest"></param>
    /// <param name="request"></param>
    /// <returns></returns>
    Task<Response<string>> CreateStaffingRequest(StaffingRequest staffingRequest, HttpRequest request);

    /// <summary>
    /// Get staffing requests
    /// </summary>
    /// <param name="skip">Number to skip</param>
    /// <param name="take">Number to take</param>
    /// <param name="status">Staffing request status</param>
    /// <returns>Staffing requests</returns>
    Task<ResponsePaging<IList<StaffingRequest>>> GetStaffingRequests(int skip, int take, StaffingRequestStatus status);

    /// <summary>
    /// Get staffing request by id
    /// </summary>
    /// <param name="staffingRequestId">Staffing request id</param>
    /// <exception cref="Shared.StaffingRequestNotFoundException">Staffing request not found</exception>
    /// <returns>Staffing request if found</returns>
    Task<Response<StaffingRequest>> GetStaffingRequest(int staffingRequestId);

    /// <summary>
    /// Update staffing request
    /// </summary>
    /// <param name="staffingRequest">Updated staffing request</param>
    /// <param name="request">Raw http request</param>
    /// <exception cref="Shared.StaffingRequestNotFoundException">Staffing request not found</exception>
    /// <returns>Updated staffing request</returns>
    Task<Response<StaffingRequest>> UpdateStaffingRequest(StaffingRequest staffingRequest, HttpRequest request);

    /// <summary>
    /// Delete staffing request
    /// </summary>
    /// <param name="staffingRequestId">Staffing request id</param>
    /// <param name="request">Raw http request</param>
    /// <exception cref="Shared.StaffingRequestNotFoundException">Staffing request not found</exception>
    /// <returns>Deleted staffing request</returns>
    Task<Response<StaffingRequest>> DeleteStaffingRequest(int staffingRequestId, HttpRequest request);
}
