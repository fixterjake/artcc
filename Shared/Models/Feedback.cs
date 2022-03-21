namespace ZDC.Shared.Models;

public class Feedback
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public User? User { get; set; }
    public string ControllerPosition { get; set; } = string.Empty;
    public FeedbackServiceLevel ServiceLevel { get; set; }
    public string Callsign { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public int Cid { get; set; }
    public FeedbackStatus Status { get; set; }
    public DateTimeOffset Timestamp { get; set; }

    public Feedback()
    {
        Status = FeedbackStatus.Pending;
        Timestamp = DateTimeOffset.UtcNow;
    }
}

public enum FeedbackServiceLevel
{
    Unsatisfactory,
    Poor,
    Fair,
    Good,
    Excellent
}

public enum FeedbackStatus
{
    Pending,
    Accepted,
    Denied
}