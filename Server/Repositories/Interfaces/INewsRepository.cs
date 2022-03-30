using ZDC.Shared.Dtos;
using ZDC.Shared.Models;

namespace ZDC.Server.Repositories.Interfaces;

public interface INewsRepository
{
    Task<Response<News>> CreateNews(News news, HttpRequest request);
    Task<Response<IList<News>>> GetNews();
    Task<Response<News>> GetNews(int newsId);
    Task<Response<News>> UpdateNews(News news, HttpRequest request);
    Task<Response<News>> DeleteNews(int newsId, HttpRequest request);
}
