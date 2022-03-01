namespace ZDC.Shared.Models;

public class Loa
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public User User { get; set; }
    public DateTime Start { get; set; }
    public DateTime End { get; set; }
    public string Reason { get; set; }
    public LoaStatus Status { get; set; }
    public DateTime Updated { get; set; }

    public Loa()
    {
        Updated = DateTime.UtcNow;
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