namespace ZDC.Shared.Models;

public class News
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Content { get; set; }
    public int UserId { get; set; }
    public User User { get; set; }
    public DateTime Updated { get; set; }

    public News()
    {
        Updated = DateTime.UtcNow;
    }
}