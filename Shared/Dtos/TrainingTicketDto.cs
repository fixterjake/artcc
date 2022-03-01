using ZDC.Shared.Models;

namespace ZDC.Shared.Dtos;

public class TrainingTicketDto
{
    public int Id { get; set; }
    public UserDto User { get; set; }
    public UserDto Trainer { get; set; }
    public TrainingFacility Facility { get; set; }
    public TrainingPosition Position { get; set; }
    public string PositionFull => $"{Facility}_{Position}";
    public DateTime Start { get; set; }
    public DateTime End { get; set; }
    public string Comments { get; set; }
    public DateTime Created { get; set; }
    public DateTime Updated { get; set; }
}