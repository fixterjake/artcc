namespace ZDC.Shared.Models;

public class WebsiteLog
{
    public int Id { get; set; }
    public string Ip { get; set; }
    public string Cid { get; set; }
    public string Name { get; set; }
    public string Action { get; set; }
    public string OldData { get; set; }
    public string NewData { get; set; }
    public DateTime Timestamp { get; set; }

    public WebsiteLog()
    {
        Timestamp = DateTime.UtcNow;
    }
}