namespace ZDC.Shared.Models;

public class TrainingTicket
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public User? User { get; set; }
    public int TrainerId { get; set; }
    public User? Trainer { get; set; }
    public TrainingFacility Facility { get; set; }
    public TrainingPosition Position { get; set; }
    public string PositionFull => $"{Facility}_{Position}";
    public DateTimeOffset Start { get; set; }
    public DateTimeOffset End { get; set; }
    public string Comments { get; set; } = string.Empty;
    public string InternalComments { get; set; } = string.Empty;
    public OtsRecommendation OtsRecommendation { get; set; }
    public DateTimeOffset Updated { get; set; }

    public TrainingTicket()
    {
        Updated = DateTimeOffset.UtcNow;
    }
}

public enum TrainingFacility
{
    IAD,
    DCA,
    BWI,
    ORF,
    RDU,
    DC
}

public enum TrainingPosition
{
    GND,
    TWR,
    APP,
    CTR
}

public enum OtsRecommendation
{
    Yes,
    No
}