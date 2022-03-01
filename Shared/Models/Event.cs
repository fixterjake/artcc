namespace ZDC.Shared.Models;

public class Event
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string Host { get; set; }
    public int UploadId { get; set; }
    public Upload Upload { get; set; }
    public DateTime Start { get; set; }
    public DateTime End { get; set; }
    public IList<EventPosition> Positions { get; set; }
    public bool Open { get; set; }
    public DateTime Updated { get; set; }

    public Event()
    {
        Updated = DateTime.UtcNow;
    }
}