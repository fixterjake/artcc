using ZDC.Shared.Dtos;
using ZDC.Shared.Models;

namespace ZDC.Server.Repositories.Interfaces;

public interface IWarningRepository
{
    /// <summary>
    /// Create a warning
    /// </summary>
    /// <param name="warning">Warning to create</param>
    /// <param name="request">Raw http request</param>
    /// <exception cref="Shared.UserNotFoundException">User or submitter not found</exception>
    /// <returns>Created warning</returns>
    Task<Response<Warning>> CreateWarning(Warning warning, HttpRequest request);

    /// <summary>
    /// Create multiple warnings
    /// </summary>
    /// <param name="warnings">Warnings to create</param>
    /// <param name="request">Raw http request</param>
    /// <returns>Created warnings</returns>
    Task<Response<IList<Warning>>> CreateWarnings(IList<Warning> warnings, HttpRequest request);

    /// <summary>
    /// Audit all members
    /// </summary>
    /// <returns>List of audit dto's that will show if they are active or not</returns>
    Task<Response<IList<AuditDto>>> AuditControllers();

    /// <summary>
    /// Get warnings from a given month and year
    /// </summary>
    /// <param name="month">Month to get</param>
    /// <param name="year">Year to get</param>
    /// <returns>Warnings from that month and year</returns>
    Task<Response<IList<Warning>>> GetWarnings(int month, int year);

    /// <summary>
    /// Delete a warning
    /// </summary>
    /// <param name="warningId">Warning to delete</param>
    /// <param name="request">Raw http request</param>
    /// <exception cref="Shared.WarningNotFoundException">Warning not found</exception>
    /// <returns>Deleted warning</returns>
    Task<Response<Warning>> DeleteWarning(int warningId, HttpRequest request);
}
