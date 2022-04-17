using ZDC.Shared.Dtos;
using ZDC.Shared.Models;

namespace ZDC.Server.Repositories.Interfaces;

public interface ISoloCertRepository
{
    Task<Response<SoloCert>> CreateSoloCert(SoloCert soloCert, HttpRequest request);
    Task<Response<IList<SoloCertDto>>> GetSoloCerts();
    Task<Response<SoloCert>> GetSoloCert(int userId);
    Task<Response<SoloCert>> ExtendSoloCert(int soloCertId, DateTimeOffset newEnd, HttpRequest request);
    Task<Response<SoloCert>> DeleteSoloCert(int soloCertId, HttpRequest request);
}
