using ZDC.Shared.Dtos;
using ZDC.Shared.Models;

namespace ZDC.Server.Repositories.Interfaces;

public interface IFeedbackRepository
{
    Task<Response<string>> CreateFeedback(Feedback feedback, HttpRequest request);
    Task<Response<IList<Feedback>>> GetFeedback();
    Task<Response<Feedback>> GetFeedback(int feedbackId);
    Task<Response<Feedback>> UpdateFeedback(Feedback feedback, HttpRequest request);
    Task<Response<Feedback>> DeleteFeedback(int feedbackId, HttpRequest request);
}
