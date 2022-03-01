using ZDC.Shared.Models;

namespace ZDC.Shared.Dtos;

public class EventRegistrationDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public UserDto User { get; set; }
    public int PositionId { get; set; }
    public EventPosition Position { get; set; }
    public int EventId { get; set; }
    public Event Event { get; set; }
    public DateTime Start { get; set; }
    public DateTime End { get; set; }
}