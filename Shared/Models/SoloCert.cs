namespace ZDC.Shared.Models;

public class SoloCert
{
    public int Id { get; set; }
    public User User { get; set; }
    public User Submitter { get; set; }
    public SoloCertFacility Position { get; set; }
    public AirportCert OldCert { get; set; }
    public AirportCert Cert { get; set; }
    public DateTime End { get; set; }
}

public enum SoloCertFacility
{
    Minor,
    Iad,
    Dca,
    Bwi,
    Dc
}