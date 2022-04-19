using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using System.Net;
using ZDC.Server.Data;
using ZDC.Server.Repositories.Interfaces;
using ZDC.Server.Services.Interfaces;
using ZDC.Shared;
using ZDC.Shared.Dtos;
using ZDC.Shared.Models;

namespace ZDC.Server.Repositories;

public class CommentRepository : ICommentRepository
{
    private readonly DatabaseContext _context;
    private readonly IDistributedCache _cache;
    private readonly ILoggingService _loggingService;

    public CommentRepository(DatabaseContext context, IDistributedCache cache, ILoggingService loggingService)
    {
        _context = context;
        _cache = cache;
        _loggingService = loggingService;
    }

    #region Create

    /// <inheritdoc />
    public async Task<Response<Comment>> CreateComment(Comment comment, HttpRequest request)
    {
        var result = await _context.Comments.AddAsync(comment);
        await _context.SaveChangesAsync();
        var newData = JsonConvert.SerializeObject(result.Entity);

        await _loggingService.AddWebsiteLog(request, $"Created comment '{result.Entity.Id}'", string.Empty, newData);

        return new Response<Comment>
        {
            StatusCode = HttpStatusCode.Created,
            Message = $"Created comment '{result.Entity.Id}'",
            Data = result.Entity
        };
    }

    #endregion

    #region Read

    /// <inheritdoc />
    public async Task<ResponsePaging<IList<Comment>>> GetUserComments(int userId, int skip, int take)
    {
        if (!_context.Users.Any(x => x.Id == userId))
            throw new UserNotFoundException($"User '{userId}' not found");
        var result = await _context.Comments
            .Where(x => x.UserId == userId)
            .Skip(skip).Take(take)
            .ToListAsync();
        var totalCount = await _context.Comments.CountAsync();

        return new ResponsePaging<IList<Comment>>
        {
            StatusCode = HttpStatusCode.OK,
            TotalCount = totalCount,
            ResultCount = result.Count,
            Message = $"Got {result.Count} comments",
            Data = result
        };
    }

    #endregion

    #region Update

    /// <inheritdoc />
    public async Task<Response<Comment>> UpdateComment(Comment comment, HttpRequest request)
    {
        var dbComment = await _context.Comments.AsNoTracking().FirstOrDefaultAsync(x => x.Id == comment.Id) ??
            throw new CommentNotFoundException($"Comment '{comment.Id}' not found");
        if (!_context.Users.Any(x => x.Id == dbComment.UserId))
            throw new UserNotFoundException($"User '{comment.UserId}' not found");
        if (!_context.Users.Any(x => x.Id == dbComment.SubmitterId))
            throw new UserNotFoundException($"User '{comment.SubmitterId}' not found");

        var oldData = JsonConvert.SerializeObject(dbComment);
        var result = _context.Comments.Update(comment);
        await _context.SaveChangesAsync();
        var newData = JsonConvert.SerializeObject(result.Entity);

        await _loggingService.AddWebsiteLog(request, $"Updated comment '{result.Entity.Id}'", oldData, newData);

        return new Response<Comment>
        {
            StatusCode = HttpStatusCode.OK,
            Message = $"Updated comment '{result.Entity.Id}'",
            Data = result.Entity
        };
    }


    #endregion

    #region Delete

    /// <inheritdoc />
    public async Task<Response<Comment>> DeleteComment(int commentId, HttpRequest request)
    {
        var comment = await _context.Comments.FindAsync(commentId) ??
            throw new CommentNotFoundException($"Comment '{commentId}' not found");

        var oldData = JsonConvert.SerializeObject(comment);
        _context.Comments.Remove(comment);
        await _context.SaveChangesAsync();

        await _loggingService.AddWebsiteLog(request, $"Deleted comment '{commentId}'", oldData, string.Empty);

        return new Response<Comment>
        {
            StatusCode = HttpStatusCode.OK,
            Message = $"Deleted comment '{commentId}'",
            Data = comment
        };
    }

    #endregion
}