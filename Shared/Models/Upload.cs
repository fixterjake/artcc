namespace ZDC.Shared.Models;

public class Upload
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Url { get; set; }
    public DateTime Timestamp { get; set; }

    public Upload()
    {
        Timestamp = DateTime.UtcNow;
    }
}