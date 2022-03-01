namespace ZDC.Shared.Models;

public class Comment
{
    public int Id { get; set; }
    public string Content { get; set; }
    public bool Confidential { get; set; }
    public int UserId { get; set; }
    public User User { get; set; }
    public int SubmitterId { get; set; }
    public User Submitter { get; set; }
    public DateTime Timestamp { get; set; }

    public Comment()
    {
        Timestamp = DateTime.UtcNow;
    }
}