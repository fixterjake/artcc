using ZDC.Shared.Dtos;
using ZDC.Shared.Models;

namespace ZDC.Server.Repositories.Interfaces;

public interface IPositionRepository
{
    /// <summary>
    /// Create zdc position
    /// </summary>
    /// <param name="position">Position to create</param>
    /// <param name="request">Raw http request</param>
    /// <returns>Created position</returns>
    Task<Response<Position>> CreatePosition(Position position, HttpRequest request);

    /// <summary>
    /// Get zdc positions
    /// </summary>
    /// <returns>Zdc positions</returns>
    Task<Response<IList<Position>>> GetPositions();

    /// <summary>
    /// Delete zdc position
    /// </summary>
    /// <param name="positionId">Position id</param>
    /// <param name="request">Raw http request</param>
    /// <exception cref="Shared.PositionNotFoundException">Position not found</exception>
    /// <returns>Deleted position</returns>
    Task<Response<Position>> DeletePosition(int positionId, HttpRequest request);
}
