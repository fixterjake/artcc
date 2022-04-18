using ZDC.Shared.Dtos;
using ZDC.Shared.Models;

namespace ZDC.Server.Repositories.Interfaces;

public interface ISoloCertRepository
{
    /// <summary>
    /// Create solo cert
    /// </summary>
    /// <param name="soloCert">Solo cert to create</param>
    /// <param name="request">Raw http request</param>
    /// <exception cref="Shared.UserNotFoundException">User not found</exception>
    /// <exception cref="Shared.SoloCertExistsException">Solo cert already exists</exception>
    /// <returns>Created solo cert</returns>
    Task<Response<SoloCert>> CreateSoloCert(SoloCert soloCert, HttpRequest request);

    /// <summary>
    /// Get solo certs
    /// </summary>
    /// <returns>Solo certs</returns>
    Task<Response<IList<SoloCertDto>>> GetSoloCerts();

    /// <summary>
    /// Get solo cert for user
    /// </summary>
    /// <param name="userId">User id</param>
    /// <exception cref="Shared.UserNotFoundException">User not found</exception>
    /// <exception cref="Shared.SoloCertNotFoundException">Solo cert not found</exception>
    /// <returns>Solo cert if found</returns>
    Task<Response<SoloCert>> GetSoloCert(int userId);

    /// <summary>
    /// Extend a solo cert
    /// </summary>
    /// <param name="soloCertId">Solo cert id</param>
    /// <param name="newEnd">New ending date</param>
    /// <param name="request">Raw http request</param>
    /// <exception cref="Shared.SoloCertNotFoundException">Solo cert not found</exception>
    /// <returns>Extended solo cert</returns>
    Task<Response<SoloCert>> ExtendSoloCert(int soloCertId, DateTimeOffset newEnd, HttpRequest request);

    /// <summary>
    /// Delete solo cert
    /// </summary>
    /// <param name="soloCertId">Solo cert id</param>
    /// <param name="request">Raw http request</param>
    /// <exception cref="Shared.SoloCertNotFoundException">Solo cert not found</exception>
    /// <returns>Deleted solo cert</returns>
    Task<Response<SoloCert>> DeleteSoloCert(int soloCertId, HttpRequest request);
}
