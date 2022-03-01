namespace ZDC.Shared.Models;

public class Feedback
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public User User { get; set; }
    public string ControllerPosition { get; set; }
    public FeedbackServiceLevel ServiceLevel { get; set; }
    public string Callsign { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public int Cid { get; set; }
    public FeedbackStatus Status { get; set; }
    public DateTime Timestamp { get; set; }

    public Feedback()
    {
        Status = FeedbackStatus.Pending;
        Timestamp = DateTime.UtcNow;
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