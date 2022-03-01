namespace ZDC.Shared.Models;

public class EmailLog
{
    public int Id { get; set; }
    public string Email { get; set; }
    public string To { get; set; }
    public DateTime Timestamp { get; set; }

    public EmailLog()
    {
        Timestamp = DateTime.UtcNow;
    }
}