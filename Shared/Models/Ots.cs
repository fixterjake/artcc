namespace ZDC.Shared.Models;

public class Ots
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public User? User { get; set; }
    public int RecommenderId { get; set; }
    public User? Recommender { get; set; }
    public int? InstructorId { get; set; }
    public User? Instructor { get; set; }
    public TrainingFacility Facility { get; set; }
    public TrainingPosition Position { get; set; }
    public OtsStatus Status { get; set; }
    public DateTimeOffset Updated { get; set; }

    public Ots()
    {
        Status = OtsStatus.Pending;
        Updated = DateTimeOffset.UtcNow;
    }
}

public enum OtsStatus
{
    Pending,
    Assigned,
    Pass,
    Fail
}