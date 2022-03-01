namespace ZDC.Shared.Models;

public class DebugLog
{
    public int Id { get; set; }
    public string Ip { get; set; }
    public string Cid { get; set; }
    public string Name { get; set; }
    public string Route { get; set; }
    public string Exception { get; set; }
    public string StackTrace { get; set; }
    public Guid Guid { get; set; }
    public DateTime Timestamp { get; set; }

    public DebugLog()
    {
        Guid = Guid.NewGuid();
        Timestamp = DateTime.UtcNow;
    }
}