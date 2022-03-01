namespace ZDC.Shared.Models;

public class EventRegistration
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public User User { get; set; }
    public int PositionId { get; set; }
    public EventPosition Position { get; set; }
    public int EventId { get; set; }
    public Event Event { get; set; }
    public DateTime Start { get; set; }
    public DateTime End { get; set; }
}

public enum EventRegistrationStatus
{
    Pending,
    Assigned,
    Relief
}