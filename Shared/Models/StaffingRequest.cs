namespace ZDC.Shared.Models;

public class StaffingRequest
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Affiliation { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTimeOffset Start { get; set; }
    public DateTimeOffset End { get; set; }
    public StaffingRequestStatus Status { get; set; }
    public DateTimeOffset Timestamp { get; set; }

    public StaffingRequest()
    {
        Status = StaffingRequestStatus.Pending;
        Timestamp = DateTimeOffset.UtcNow;
    }
}

public enum StaffingRequestStatus
{
    Pending,
    Accepted,
    Denied
}