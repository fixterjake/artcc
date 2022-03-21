namespace ZDC.Shared.Models;

public class DebugLog
{
    public int Id { get; set; }
    public string Ip { get; set; } = string.Empty;
    public string Cid { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Route { get; set; } = string.Empty;
    public string Exception { get; set; } = string.Empty;
    public string StackTrace { get; set; } = string.Empty;
    public Guid Guid { get; set; }
    public DateTimeOffset Timestamp { get; set; }

    public DebugLog()
    {
        Guid = Guid.NewGuid();
        Timestamp = DateTimeOffset.UtcNow;
    }
}