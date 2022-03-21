using ZDC.Shared.Models;

namespace ZDC.Shared.Dtos;

public class UserDto
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string FullName => $"{FirstName} {LastName}";
    public string FullNameCid => $"{FirstName} {LastName} - {Id}";
    public string ReverseNameCid => $"{LastName}, {FirstName} - {Id}";
    public string Initials { get; set; } = string.Empty;
    public DateTimeOffset Joined { get; set; }
    public bool Visitor { get; set; }
    public string VisitorFrom { get; set; } = string.Empty;
    public Rating Rating { get; set; }
    public AirportCert Minor { get; set; }
    public AirportCert Iad { get; set; }
    public AirportCert Dca { get; set; }
    public AirportCert Bwi { get; set; }
    public CenterCert Center { get; set; }
    public UserStatus Status { get; set; }
    public ICollection<Role>? Roles { get; set; }
    public DateTimeOffset Created { get; set; }
    public DateTimeOffset Updated { get; set; }
}