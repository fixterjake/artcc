using ZDC.Shared.Dtos;
using ZDC.Shared.Models;

namespace ZDC.Server.Repositories.Interfaces;

public interface IFeedbackRepository
{
    /// <summary>
    /// Create feedback
    /// </summary>
    /// <param name="feedback">Feedback to create</param>
    /// <param name="request">Raw http request</param>
    /// <exception cref="Shared.UserNotFoundException">User not found</exception>
    /// <returns>Message stating feedback was created, this is public facing so no return object</returns>
    Task<Response<string>> CreateFeedback(Feedback feedback, HttpRequest request);

    /// <summary>
    /// Get all feedback
    /// </summary>
    /// <param name="skip">Number to skip</param>
    /// <param name="take">Number to take</param>
    /// <returns>Feedback</returns>
    Task<Response<IList<Feedback>>> GetFeedback(int skip, int take);

    /// <summary>
    /// Get feedback by id
    /// </summary>
    /// <param name="feedbackId">Feedback id</param>
    /// <exception cref="Shared.FeedbackNotFoundException">Feedback not found</exception>
    /// <returns>Feedback if found</returns>
    Task<Response<Feedback>> GetFeedback(int feedbackId);

    /// <summary>
    /// Updated feedback
    /// </summary>
    /// <param name="feedback">Updated feedback</param>
    /// <param name="request">Raw http request</param>
    /// <exception cref="Shared.FeedbackNotFoundException">Feedback not found</exception>
    /// <exception cref="Shared.UserNotFoundException">User not found</exception>
    /// <returns>Updated feedback</returns>
    Task<Response<Feedback>> UpdateFeedback(Feedback feedback, HttpRequest request);

    /// <summary>
    /// Delete feedback
    /// </summary>
    /// <param name="feedbackId">Feedback id</param>
    /// <param name="request">Raw http request</param>
    /// <exception cref="Shared.FeedbackNotFoundException">Feedback not found</exception>
    /// <returns>Deleted feedback</returns>
    Task<Response<Feedback>> DeleteFeedback(int feedbackId, HttpRequest request);
}
