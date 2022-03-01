using Microsoft.AspNetCore.Mvc;

namespace ZDC.Server.Services.Interfaces;

public interface ILoggingService
{
    Task AddWebsiteLog(HttpRequest request, string action, string oldData, string newData);
    Task<ActionResult> AddDebugLog(HttpRequest request, string route, string exception, string stackTrace);
}