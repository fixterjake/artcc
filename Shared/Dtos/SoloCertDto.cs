using ZDC.Shared.Models;

namespace ZDC.Shared.Dtos;

public class SoloCertDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public UserDto? User { get; set; }
    public int SubmittedId { get; set; }
    public UserDto? Submitter { get; set; }
    public SoloCertFacility Position { get; set; }
    public int OldCert { get; set; }
    public int Cert { get; set; }
    public string PositionString { get; set; } = string.Empty;
    public DateTimeOffset End { get; set; }
}