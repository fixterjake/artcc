namespace ZDC.Shared.Models;

public class Settings
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Value { get; set; }
    public DateTime Updated { get; set; }

    public Settings()
    {
        Updated = DateTime.UtcNow;
    }
}