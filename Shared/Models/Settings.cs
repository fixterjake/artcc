namespace ZDC.Shared.Models;

public class Settings
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public DateTimeOffset Updated { get; set; }

    public Settings()
    {
        Updated = DateTimeOffset.UtcNow;
    }
}