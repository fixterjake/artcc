namespace ZDC.Shared.Models;

public class File
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Version { get; set; } = string.Empty;
    public FileCategory Category { get; set; }
    public int UploadId { get; set; }
    public Upload? Upload { get; set; }
    public DateTimeOffset Updated { get; set; }

    public File()
    {
        Updated = DateTimeOffset.UtcNow;
    }
}

public enum FileCategory
{
    VRC,
    vSTARS,
    vERAM,
    vATIS,
    Euroscope,
    SOPs,
    LOAs,
    TrainingStaff,
    Staff
}