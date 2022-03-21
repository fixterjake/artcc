namespace ZDC.Shared.Models;

public class EmailLog
{
    public int Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string To { get; set; } = string.Empty;
    public DateTimeOffset Timestamp { get; set; }

    public EmailLog()
    {
        Timestamp = DateTimeOffset.UtcNow;
    }
}