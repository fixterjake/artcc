namespace ZDC.Shared.Models;

public class Announcement
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public int UserId { get; set; }
    public string User { get; set; } = string.Empty;
    public DateTimeOffset Updated { get; set; }

    public Announcement()
    {
        Updated = DateTimeOffset.UtcNow;
    }
}