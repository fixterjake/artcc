using ZDC.Shared.Dtos;
using ZDC.Shared.Models;

namespace ZDC.Server.Repositories.Interfaces;

public interface IEmailLogRepository
{
    /// <summary>
    /// Get email logs
    /// </summary>
    /// <param name="skip">Number to skip</param>
    /// <param name="take">Number to take</param>
    /// <returns>Email logs</returns>
    Task<ResponsePaging<IList<EmailLog>>> GetEmailLogs(int skip, int take);
}
