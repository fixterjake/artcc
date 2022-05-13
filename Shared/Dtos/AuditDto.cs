using ZDC.Shared.Models;

namespace ZDC.Shared.Dtos;

public class AuditDto
{
    public User? User { get; set; }
    public int Month { get; set; }
    public int Year { get; set; }
    public IList<Hours>? SixMonthHours { get; set; }
}
