namespace ZDC.Jobs.Services.Interfaces;

public interface ILoggingService
{
    Task AddWebsiteLog(string action, string oldData, string newData);
}
