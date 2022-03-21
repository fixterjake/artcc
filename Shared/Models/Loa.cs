namespace ZDC.Shared.Models;

public class Loa
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public User? User { get; set; }
    public DateTime Start { get; set; }
    public DateTime End { get; set; }
    public string Reason { get; set; } = string.Empty;
    public LoaStatus Status { get; set; }
    public DateTimeOffset Updated { get; set; }

    public Loa()
    {
        Updated = DateTimeOffset.UtcNow;
    }
}

public enum LoaStatus
{
    Pending,
    Approved,
    Denied,
    Started,
    Ended
}