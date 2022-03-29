using ZDC.Shared.Dtos;
using ZDC.Shared.Models;

namespace ZDC.Server.Repositories.Interfaces;

public interface IAirportRepository
{
    Task<Response<Airport>> CreateAirport(Airport airport, HttpRequest request);
    Task<Response<IList<Airport>>> GetAirports();
    Task<Response<Airport>> GetAirport(int airportId);
    Task<Response<Airport>> UpdateAirport(Airport airport, HttpRequest request);
    Task<Response<Airport>> DeleteAirport(int airportId, HttpRequest request);
}