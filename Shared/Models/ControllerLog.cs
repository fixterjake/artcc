namespace ZDC.Shared.Models;

public class ControllerLog
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public User User { get; set; }
    public string Callsign { get; set; }
    public string Frequency { get; set; }
    public DateTime Start { get; set; }
    public DateTime End { get; set; }
    public TimeSpan Duration { get; set; }
}