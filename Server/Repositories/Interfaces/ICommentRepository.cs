using ZDC.Shared.Dtos;
using ZDC.Shared.Models;

namespace ZDC.Server.Repositories.Interfaces;

public interface ICommentRepository
{
    /// <summary>
    /// Create a comment
    /// </summary>
    /// <param name="comment">Comment to create</param>
    /// <param name="request">Raw http request</param>
    /// <returns>Created comment</returns>
    Task<Response<Comment>> CreateComment(Comment comment, HttpRequest request);

    /// <summary>
    /// Get a users comments
    /// </summary>
    /// <param name="userId">User id to get comments</param>
    /// <param name="skip">Number to skip</param>
    /// <param name="take">Number to take</param>
    /// <exception cref="Shared.UserNotFoundException">User not found</exception>
    /// <returns>User comments</returns>
    Task<ResponsePaging<IList<Comment>>> GetUserComments(int userId, int skip, int take);

    /// <summary>
    /// Update a comment
    /// </summary>
    /// <param name="comment">Updated comment</param>
    /// <param name="request">Raw http request</param>
    /// <exception cref="Shared.CommentNotFoundException">Comment not found</exception>
    /// <exception cref="Shared.UserNotFoundException">User not found</exception>
    /// <returns>Updated comment</returns>
    Task<Response<Comment>> UpdateComment(Comment comment, HttpRequest request);

    /// <summary>
    /// Delete a comment
    /// </summary>
    /// <param name="commentId">Comment id</param>
    /// <param name="request">Raw http request</param>
    /// <exception cref="Shared.CommentNotFoundException">Comment not found</exception>
    /// <returns>Deleted comment</returns>
    Task<Response<Comment>> DeleteComment(int commentId, HttpRequest request);
}