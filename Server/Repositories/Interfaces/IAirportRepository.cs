using ZDC.Shared.Dtos;
using ZDC.Shared.Models;

namespace ZDC.Server.Repositories.Interfaces;

public interface IAirportRepository
{
    /// <summary>
    /// Create a new airport
    /// </summary>
    /// <param name="airport">Airport to create</param>
    /// <param name="request">Raw http request</param>
    /// <returns>Created airport</returns>
    Task<Response<Airport>> CreateAirport(Airport airport, HttpRequest request);

    /// <summary>
    /// Get all airports
    /// </summary>
    /// <returns>All airports</returns>
    Task<Response<IList<Airport>>> GetAirports();

    /// <summary>
    /// Get airport by id
    /// </summary>
    /// <param name="airportId">Airport id to get</param>
    /// <exception cref="Shared.AirportNotFoundException">Airport not found</exception>
    /// <returns>Airport if found</returns>
    Task<Response<Airport>> GetAirport(int airportId);

    /// <summary>
    /// Update an airport
    /// </summary>
    /// <param name="airport">Updated airport</param>
    /// <param name="request">Raw http request</param>
    /// <exception cref="Shared.AirportNotFoundException">Airport not found</exception>
    /// <returns>Updated airport</returns>
    Task<Response<Airport>> UpdateAirport(Airport airport, HttpRequest request);

    /// <summary>
    /// Delete an airport
    /// </summary>
    /// <param name="airportId">Airport id to delete</param>
    /// <param name="request">Raw http request</param>
    /// <exception cref="Shared.AirportNotFoundException">Airport not found</exception>
    /// <returns>Deleted airport</returns>
    Task<Response<Airport>> DeleteAirport(int airportId, HttpRequest request);
}