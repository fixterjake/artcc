namespace ZDC.Shared.Models;

public class WebsiteLog
{
    public int Id { get; set; }
    public string Ip { get; set; } = string.Empty;
    public string Cid { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty;
    public string OldData { get; set; } = string.Empty;
    public string NewData { get; set; } = string.Empty;
    public DateTimeOffset Timestamp { get; set; }

    public WebsiteLog()
    {
        Timestamp = DateTimeOffset.UtcNow;
    }
}