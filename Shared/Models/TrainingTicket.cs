namespace ZDC.Shared.Models;

public class TrainingTicket
{
    public int Id { get; set; }
    public User User { get; set; }
    public User Trainer { get; set; }
    public TrainingFacility Facility { get; set; }
    public TrainingPosition Position { get; set; }
    public string PositionFull => $"{Facility}_{Position}";
    public DateTime Start { get; set; }
    public DateTime End { get; set; }
    public string Comments { get; set; }
    public string InternalComments { get; set; }
    public OtsRecommendation OtsRecommendation { get; set; }
    public DateTime Updated { get; set; }

    public TrainingTicket()
    {
        Updated = DateTime.UtcNow;
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