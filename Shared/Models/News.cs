namespace ZDC.Shared.Models;

public class News
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public int UserId { get; set; }
    public User? User { get; set; }
    public DateTimeOffset Updated { get; set; }

    public News()
    {
        Updated = DateTimeOffset.UtcNow;
    }
}