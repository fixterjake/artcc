using ZDC.Shared.Dtos;
using ZDC.Shared.Models;

namespace ZDC.Server.Repositories.Interfaces;

public interface ILoaRepository
{
    Task<Response<Loa>> CreateLoa(Loa loa, HttpRequest request);
    Task<Response<IList<Loa>>> GetLoas();
    Task<Response<Loa>> GetLoa(int idloaId);
    Task<Response<Loa>> UpdateLoa(Loa loa, HttpRequest request);
    Task<Response<Loa>> DeleteLoa(int loaId, HttpRequest request);
}
