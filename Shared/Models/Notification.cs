namespace ZDC.Shared.Models;

public class Notification
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Link { get; set; } = string.Empty;
    public bool Read { get; set; } = false;
    public DateTimeOffset Timestamp { get; set; }

    public Notification()
    {
        Timestamp = DateTimeOffset.UtcNow;
    }
}