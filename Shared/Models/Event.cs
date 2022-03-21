namespace ZDC.Shared.Models;

public class Event
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Host { get; set; } = string.Empty;
    public int UploadId { get; set; }
    public Upload? Upload { get; set; }
    public DateTimeOffset Start { get; set; }
    public DateTimeOffset End { get; set; }
    public IList<EventPosition>? Positions { get; set; }
    public bool Open { get; set; }
    public DateTimeOffset Updated { get; set; }

    public Event()
    {
        Updated = DateTimeOffset.UtcNow;
    }
}