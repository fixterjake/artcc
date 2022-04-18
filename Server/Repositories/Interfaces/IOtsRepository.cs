using ZDC.Shared.Dtos;
using ZDC.Shared.Models;

namespace ZDC.Server.Repositories.Interfaces;

public interface IOtsRepository
{
    /// <summary>
    /// Create ots
    /// </summary>
    /// <param name="ots">Ots to create</param>
    /// <param name="request">Raw http request</param>
    /// <exception cref="Shared.UserNotFoundException">User not found</exception>
    /// <returns>Created ots</returns>
    Task<Response<Ots>> CreateOts(Ots ots, HttpRequest request);

    /// <summary>
    /// Get ots's
    /// </summary>
    /// <returns>Ots's</returns>
    Task<Response<IList<Ots>>> GetOts(int skip, int take, OtsStatus status);

    /// <summary>
    /// Get ots by id
    /// </summary>
    /// <param name="otsId">Ots id</param>
    /// <exception cref="Shared.OtsNotFoundException">Ots not found</exception>
    /// <returns>Ots if found</returns>
    Task<Response<Ots>> GetOts(int otsId);

    /// <summary>
    /// Update ots
    /// </summary>
    /// <param name="ots">Updated ots</param>
    /// <param name="request">Raw http request</param>
    /// <exception cref="Shared.OtsNotFoundException">Ots not found</exception>
    /// <exception cref="Shared.UserNotFoundException">User not found</exception>
    /// <returns>Updated ots</returns>
    Task<Response<Ots>> UpdateOts(Ots ots, HttpRequest request);

    /// <summary>
    /// Delete ots
    /// </summary>
    /// <param name="otsId">Ots id</param>
    /// <param name="request">Raw http request</param>
    /// <exception cref="Shared.OtsNotFoundException">Ots not found</exception>
    /// <returns>Deleted ots</returns>
    Task<Response<Ots>> DeleteOts(int otsId, HttpRequest request);
}
