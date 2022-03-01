namespace ZDC.Shared.Models;

public class File
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string Version { get; set; }
    public FileCategory Category { get; set; }
    public int UploadId { get; set; }
    public Upload Upload { get; set; }
    public DateTime Updated { get; set; }

    public File()
    {
        Updated = DateTime.UtcNow;
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