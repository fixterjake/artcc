using ZDC.Shared.Models;

namespace ZDC.Server.Repositories.Interfaces;

public interface IEmailLogRepository
{
    Task<IList<EmailLog>> GetEmailLogs(int skip, int take);
}
