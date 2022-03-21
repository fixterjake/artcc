namespace ZDC.Shared.Models;

public class ControllerLog
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public User? User { get; set; }
    public string Callsign { get; set; } = string.Empty;
    public string Frequency { get; set; } = string.Empty;
    public DateTimeOffset Start { get; set; }
    public DateTimeOffset End { get; set; }
    public TimeSpan Duration { get; set; }
    public ControllerLogType Type { get; set; }
}

public enum ControllerLogType
{
    Local,
    Tracon,
    Center
}