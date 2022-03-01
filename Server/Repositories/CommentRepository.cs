using System.Net;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
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
    private readonly ILoggingService _loggingService;

    public CommentRepository(DatabaseContext context, ILoggingService loggingService)
    {
        _context = context;
        _loggingService = loggingService;
    }

    public async Task<Response<Comment>> CreateComment(Comment comment, HttpRequest request)
    {
        var result = await _context.Comments.AddAsync(comment);
        await _context.SaveChangesAsync();
        var newData = JsonConvert.SerializeObject(result.Entity);

        await _loggingService.AddWebsiteLog(request, $"Created comment '{result.Entity.Id}'", string.Empty, newData);

        return new Response<Comment>
        {
            StatusCode = HttpStatusCode.OK,
            Message = $"Created comment '{result.Entity.Id}'",
            Data = result.Entity
        };
    }

    public async Task<Response<IList<Comment>>> GetUserComments(int id)
    {
        if (!_context.Users.Any(x => x.Id == id))
            throw new UserNotFoundException($"User '{id}' not found");

        var comments = await _context.Comments.Where(x => x.UserId == id).ToListAsync();
        return new Response<IList<Comment>>
        {
            StatusCode = HttpStatusCode.OK,
            Message = $"Got {comments.Count} comments",
            Data = comments
        };
    }

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

    public async Task<Response<Comment>> DeleteComment(int id, HttpRequest request)
    {
        var comment = await _context.Comments.FindAsync(id) ??
                      throw new CommentNotFoundException($"Comment '{id}' not found");

        var oldData = JsonConvert.SerializeObject(comment);
        _context.Comments.Remove(comment);
        await _context.SaveChangesAsync();

        await _loggingService.AddWebsiteLog(request, $"Deleted comment '{id}'", oldData, string.Empty);

        return new Response<Comment>
        {
            StatusCode = HttpStatusCode.OK,
            Message = $"Deleted comment '{id}'",
            Data = comment
        };
    }
}