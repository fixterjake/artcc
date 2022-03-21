using ZDC.Shared.Models;

namespace ZDC.Shared.Dtos;

public class TrainingTicketDto
{
    public int Id { get; set; }
    public UserDto? User { get; set; }
    public UserDto? Trainer { get; set; }
    public TrainingFacility Facility { get; set; }
    public TrainingPosition Position { get; set; }
    public string PositionFull => $"{Facility}_{Position}";
    public DateTimeOffset Start { get; set; }
    public DateTimeOffset End { get; set; }
    public string Comments { get; set; } = string.Empty;
    public DateTimeOffset Created { get; set; }
    public DateTimeOffset Updated { get; set; }
}