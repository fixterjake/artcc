namespace ZDC.Server.Services.Interfaces;

public interface ILoggingService
{
    Task AddWebsiteLog(HttpRequest request, string action, string oldData, string newData);
}