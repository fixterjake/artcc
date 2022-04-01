using ZDC.Shared.Dtos;
using ZDC.Shared.Models;

namespace ZDC.Server.Repositories.Interfaces;

public interface IOtsRepository
{
    Task<Response<Ots>> CreateOts(Ots ots, HttpRequest request);
    Task<Response<IList<Ots>>> GetOts();
    Task<Response<Ots>> GetOts(int otsId);
    Task<Response<Ots>> UpdateOts(Ots ots, HttpRequest request);
    Task<Response<Ots>> DeleteOts(int otsId, HttpRequest request);
}
