using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Net;
using ZDC.Server.Data;
using ZDC.Server.Extensions;
using ZDC.Server.Repositories.Interfaces;
using ZDC.Server.Services.Interfaces;
using ZDC.Shared;
using ZDC.Shared.Dtos;
using ZDC.Shared.Models;

namespace ZDC.Server.Repositories;

public class NewsRepository : INewsRepository
{
    private readonly DatabaseContext _context;
    private readonly ILoggingService _loggingService;

    public NewsRepository(DatabaseContext context, ILoggingService loggingService)
    {
        _context = context;
        _loggingService = loggingService;
    }

    #region Create

    public async Task<Response<News>> CreateNews(News news, HttpRequest request)
    {
        var user = await request.HttpContext.GetUser(_context) ??
            throw new UserNotFoundException("User not found");

        news.UserId = user.Id;
        news.User = user.FullName;

        var result = await _context.News.AddAsync(news);
        await _context.SaveChangesAsync();
        var newData = JsonConvert.SerializeObject(result.Entity);

        await _loggingService.AddWebsiteLog(request, $"Created news '{result.Entity.Id}'", string.Empty, newData);

        return new Response<News>
        {
            StatusCode = HttpStatusCode.Created,
            Message = $"Created news '{result.Entity.Id}'",
            Data = result.Entity
        };
    }

    #endregion

    #region Read

    public async Task<Response<IList<News>>> GetNews()
    {
        var news = await _context.News.ToListAsync();
        return new Response<IList<News>>
        {
            StatusCode = HttpStatusCode.OK,
            Message = $"Got {news.Count} news",
            Data = news
        };
    }

    public async Task<Response<News>> GetNews(int newsId)
    {
        var news = await _context.News.FindAsync(newsId) ??
            throw new NewsNotFoundException($"News '{newsId}' not found");

        return new Response<News>
        {
            StatusCode = HttpStatusCode.OK,
            Message = $"Got news '{news.Id}'",
            Data = news
        };
    }

    #endregion

    #region Update

    public async Task<Response<News>> UpdateNews(News news, HttpRequest request)
    {
        var user = await request.HttpContext.GetUser(_context) ??
            throw new UserNotFoundException("User not found");
        var dbNews = await _context.News.FindAsync(news.Id) ??
            throw new NewsNotFoundException($"News '{news.Id}' not found");

        news.User = user.FullName;

        var oldData = JsonConvert.SerializeObject(dbNews);
        var result = _context.News.Update(news);
        await _context.SaveChangesAsync();
        var newData = JsonConvert.SerializeObject(result.Entity);

        await _loggingService.AddWebsiteLog(request, $"Updated news '{result.Entity.Id}'", oldData, newData);

        return new Response<News>
        {
            StatusCode = HttpStatusCode.OK,
            Message = $"Updated news '{result.Entity.Id}'",
            Data = result.Entity
        };
    }

    #endregion

    #region Delete

    public async Task<Response<News>> DeleteNews(int newsId, HttpRequest request)
    {
        var news = await _context.News.FindAsync(newsId) ??
            throw new NewsNotFoundException($"News '{newsId}' not found");

        var oldData = JsonConvert.SerializeObject(news);
        _context.News.Remove(news);
        await _context.SaveChangesAsync();

        await _loggingService.AddWebsiteLog(request, $"Deleted news '{newsId}'", oldData, string.Empty);

        return new Response<News>
        {
            StatusCode = HttpStatusCode.OK,
            Message = $"Deleted news '{newsId}'",
            Data = news
        };
    }

    #endregion
}
