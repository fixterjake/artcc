using ZDC.Shared.Dtos;
using ZDC.Shared.Models;

namespace ZDC.Server.Repositories.Interfaces;

public interface IPositionRepository
{
    Task<Response<Position>> CreatePosition(Position position, HttpRequest request);
    Task<Response<IList<Position>>> GetPositions();
    Task<Response<Position>> DeletePosition(int positionId, HttpRequest request);
}
