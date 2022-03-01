namespace ZDC.Shared.Models;

public class StaffingRequest
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string Affiliation { get; set; }
    public string Description { get; set; }
    public DateTime Start { get; set; }
    public DateTime End { get; set; }
    public DateTime Timestamp { get; set; }

    public StaffingRequest()
    {
        Timestamp = DateTime.UtcNow;
    }
}