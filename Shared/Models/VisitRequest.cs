namespace ZDC.Shared.Models;

public class VisitRequest
{
    public int Id { get; set; }
    public int Cid { get; set; }
    public string Name { get; set; }
    public Rating Rating { get; set; }
    public string VisitReason { get; set; }
    public DateTime LastRatingChange { get; set; }
    public double RatingHours { get; set; }
    public VisitRequestStatus Status { get; set; }
    public DateTime Updated { get; set; }

    public VisitRequest()
    {
        Updated = DateTime.UtcNow;
    }
}

public enum VisitRequestStatus 
{
    Pending,
    Approved,
    Rejected
}