using ZDC.Shared.Dtos;
using ZDC.Shared.Models;

namespace ZDC.Server.Repositories.Interfaces;

public interface ICommentRepository
{
    Task<Response<Comment>> CreateComment(Comment comment, HttpRequest request);
    Task<Response<IList<Comment>>> GetUserComments(int userId);
    Task<Response<Comment>> UpdateComment(Comment comment, HttpRequest request);
    Task<Response<Comment>> DeleteComment(int commentId, HttpRequest request);
}