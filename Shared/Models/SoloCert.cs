namespace ZDC.Shared.Models;

public class SoloCert
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public User? User { get; set; }
    public int SubmitterId { get; set; }
    public User? Submitter { get; set; }
    public SoloCertFacility Position { get; set; }
    public int OldCert { get; set; }
    public int Cert { get; set; }
    public DateTimeOffset Start { get; set; }
    public DateTimeOffset End { get; set; }
    public DateTimeOffset Timestamp { get; set; }

    public SoloCert()
    {
        Timestamp = DateTimeOffset.UtcNow;
    }
}

public enum SoloCertFacility
{
    Minor,
    Iad,
    Dca,
    Bwi,
    Dc
}