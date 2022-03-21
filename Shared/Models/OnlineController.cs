namespace ZDC.Shared.Models;

public class OnlineController
{
    public int Id { get; set; }
    public string User { get; set; } = string.Empty;
    public string Callsign { get; set; } = string.Empty;
    public string Frequency { get; set; } = string.Empty;
    public string Duration { get; set; } = string.Empty;
}