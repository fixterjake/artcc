namespace ZDC.Shared.Models;

public class Upload
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public DateTimeOffset Timestamp { get; set; }

    public Upload()
    {
        Timestamp = DateTimeOffset.UtcNow;
    }
}