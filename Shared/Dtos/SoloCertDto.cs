using ZDC.Shared.Models;

namespace ZDC.Shared.Dtos;

public class SoloCertDto
{
    public int Id { get; set; }
    public UserDto User { get; set; }
    public UserDto Submitter { get; set; }
    public SoloCertFacility Position { get; set; }
    public AirportCert OldCert { get; set; }
    public AirportCert Cert { get; set; }
    public DateTime End { get; set; }
}