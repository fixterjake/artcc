namespace ZDC.Shared.Models;

public class User
{
    public int Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string FullName => $"{FirstName} {LastName}";
    public string FullNameCid => $"{FirstName} {LastName} - {Id}";
    public string ReverseNameCid => $"{LastName}, {FirstName} - {Id}";
    public string Initials { get; set; }
    public string Email { get; set; }
    public DateTime Joined { get; set; }
    public bool Visitor { get; set; }
    public string VisitorFrom { get; set; }
    public Rating Rating { get; set; }
    public AirportCert Minor { get; set; }
    public AirportCert Iad { get; set; }
    public AirportCert Dca { get; set; }
    public AirportCert Bwi { get; set; }
    public CenterCert Center { get; set; }
    public UserStatus Status { get; set; }
    public ICollection<Role> Roles { get; set; }
    public DateTime Updated { get; set; }

    public User()
    {
        Minor = AirportCert.None;
        Iad = AirportCert.None;
        Dca = AirportCert.None;
        Bwi = AirportCert.None;
        Center = CenterCert.None;
        Status = UserStatus.Active;
        Updated = DateTime.UtcNow;
    }
}

public enum Rating
{
    INAC = -1,
    SUS,
    OBS,
    S1,
    S2,
    S3,
    C1,
    C2,
    C3,
    I1,
    I2,
    I3,
    SUP,
    ADM
}

public enum AirportCert
{
    None,
    SoloGround,
    Ground,
    SoloTower,
    Tower,
    SoloApproach,
    Approach
}

public enum CenterCert
{
    None,
    Solo,
    Certified
}

public enum UserStatus
{
    Active,
    Inactive,
    Loa,
    Exempt,
    Removed
}