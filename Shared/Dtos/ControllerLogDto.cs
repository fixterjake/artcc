using ZDC.Shared.Models;

namespace ZDC.Shared.Dtos;

public class ControllerLogDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string Callsign { get; set; } = string.Empty;
    public string Frequency { get; set; } = string.Empty;
    public DateTimeOffset Start { get; set; }
    public DateTimeOffset End { get; set; }
    public TimeSpan Duration { get; set; }
    public ControllerLogType Type { get; set; }
}
