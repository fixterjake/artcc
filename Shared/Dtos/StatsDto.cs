using ZDC.Shared.Models;

namespace ZDC.Shared.Dtos;

public class StatsDto
{
    public string FullName { get; set; } = string.Empty;
    public string RatingLong { get; set; } = string.Empty;
    public UserStatus Status { get; set; }
    public bool Visitor { get; set; }
    public HoursDto? Hours { get; set; }
}
