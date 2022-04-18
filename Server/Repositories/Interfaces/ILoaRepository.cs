using ZDC.Shared.Dtos;
using ZDC.Shared.Models;

namespace ZDC.Server.Repositories.Interfaces;

public interface ILoaRepository
{
    /// <summary>
    /// Create loa
    /// </summary>
    /// <param name="loa">Loa to create</param>
    /// <param name="request">Raw http request</param>
    /// <returns>Created loa</returns>
    Task<Response<Loa>> CreateLoa(Loa loa, HttpRequest request);

    /// <summary>
    /// Get loas
    /// </summary>
    /// <returns>Loas</returns>
    Task<Response<IList<Loa>>> GetLoas(int skip, int take);

    /// <summary>
    /// Get loa by id
    /// </summary>
    /// <param name="loaId">Loa id</param>
    /// <exception cref="Shared.LoaNotFoundException">Loa not found</exception>
    /// <returns>Loa if found</returns>
    Task<Response<Loa>> GetLoa(int loaId);

    /// <summary>
    /// Update loa
    /// </summary>
    /// <param name="loa">Updated loa</param>
    /// <param name="request">Raw http request</param>
    /// <exception cref="Shared.LoaNotFoundException">Loa not found</exception>
    /// <returns>Updated loa</returns>
    Task<Response<Loa>> UpdateLoa(Loa loa, HttpRequest request);

    /// <summary>
    /// Delete loa
    /// </summary>
    /// <param name="loaId">Loa id</param>
    /// <param name="request">Raw http request</param>
    /// <exception cref="Shared.LoaNotFoundException">Loa not found</exception>
    /// <returns>Deleted loa</returns>
    Task<Response<Loa>> DeleteLoa(int loaId, HttpRequest request);
}
