namespace ZDC.Shared.Models;

public class VisitRequest
{
    public int Id { get; set; }
    public int Cid { get; set; }
    public string Name { get; set; } = string.Empty;
    public Rating Rating { get; set; }
    public string VisitReason { get; set; } = string.Empty;
    public DateTimeOffset LastRatingChange { get; set; }
    public double RatingHours { get; set; }
    public VisitRequestStatus Status { get; set; }
    public DateTimeOffset Updated { get; set; }

    public VisitRequest()
    {
        Updated = DateTimeOffset.UtcNow;
    }
}

public enum VisitRequestStatus
{
    Pending,
    Approved,
    Rejected
}