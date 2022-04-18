using ZDC.Shared.Dtos;
using ZDC.Shared.Models;

namespace ZDC.Server.Repositories.Interfaces;

public interface ITrainingTicketRepository
{
    /// <summary>
    /// Create training request
    /// </summary>
    /// <param name="trainingTicket">Training ticket to create</param>
    /// <param name="request">Raw http request</param>
    /// <exception cref="Shared.UserNotFoundException">User not found</exception>
    /// <returns>Created training ticket</returns>
    Task<Response<TrainingTicket>> CreateTrainingTicket(TrainingTicket trainingTicket, HttpRequest request);

    /// <summary>
    /// Get training tickets
    /// </summary>
    /// <param name="skip">Number to skip<param>
    /// <param name="take">Number to take</param>
    /// <returns>Training tickets</returns>
    Task<ResponsePaging<IList<TrainingTicket>>> GetTrainingTickets(int skip, int take);

    /// <summary>
    /// Get user training tickets
    /// </summary>
    /// <param name="userId">User id</param>
    /// <param name="skip">Number to skip</param>
    /// <param name="take">Number to take</param>
    /// <exception cref="Shared.UserNotFoundException">User not found</exception>
    /// <returns>User training tickets</returns>
    Task<ResponsePaging<IList<TrainingTicket>>> GetUserTrainingTickets(int userId, int skip, int take);

    /// <summary>
    /// Get user training tickets
    /// </summary>
    /// <param name="skip">Number to skip</param>
    /// <param name="take">Number to take</param>
    /// <param name="request">Raw http request</param>
    /// <exception cref="Shared.UserNotFoundException">User not found</exception>
    /// <returns>User training tickets</returns>
    Task<ResponsePaging<IList<TrainingTicketDto>>> GetUserTrainingTickets(int skip, int take, HttpRequest request);

    /// <summary>
    /// Get training ticket by id
    /// </summary>
    /// <param name="trainingTicketId">Training ticket id</param>
    /// <exception cref="Shared.TrainingTicketNotFoundException">Training ticket not found</exception>
    /// <returns>Training ticket if found</returns>
    Task<Response<TrainingTicket>> GetTrainingTicket(int trainingTicketId);

    /// <summary>
    /// Update training ticket
    /// </summary>
    /// <param name="trainingTicket">Updated training ticket</param>
    /// <param name="request">Raw http request</param>
    /// <exception cref="Shared.TrainingTicketNotFoundException">Training ticket not found</exception>
    /// <exception cref="Shared.UserNotFoundException">User not found</exception>
    /// <returns>Updated training ticket</returns>
    Task<Response<TrainingTicket>> UpdateTrainingTicket(TrainingTicket trainingTicket, HttpRequest request);

    /// <summary>
    /// Delete training ticket
    /// </summary>
    /// <param name="trainingTicketId">Training ticket id</param>
    /// <param name="request">Raw http request</param>
    /// <exception cref="Shared.TrainingTicketNotFoundException">Training ticket not found</exception>
    /// <returns>Deleted training ticket</returns>
    Task<Response<TrainingTicket>> DeleteTrainingTicket(int trainingTicketId, HttpRequest request);
}
