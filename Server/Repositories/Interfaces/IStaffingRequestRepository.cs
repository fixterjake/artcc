using ZDC.Shared.Dtos;
using ZDC.Shared.Models;

namespace ZDC.Server.Repositories.Interfaces;

public interface IStaffingRequestRepository
{
    Task<Response<string>> CreateStaffingRequest(StaffingRequest staffingRequest, HttpRequest request);
    Task<Response<IList<StaffingRequest>>> GetStaffingRequests();
    Task<Response<StaffingRequest>> GetStaffingRequest(int staffingRequestId);
    Task<Response<StaffingRequest>> UpdateStaffingRequest(StaffingRequest staffingRequest, HttpRequest request);
    Task<Response<StaffingRequest>> DeleteStaffingRequest(int staffingRequestId, HttpRequest request);
}
