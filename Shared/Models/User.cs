namespace ZDC.Shared.Models;

public class User
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string FullName => $"{FirstName} {LastName}";
    public string FullNameCid => $"{FirstName} {LastName} - {Id}";
    public string ReverseNameCid => $"{LastName}, {FirstName} - {Id}";
    public string Initials { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateTimeOffset Joined { get; set; }
    public bool Visitor { get; set; }
    public string? VisitorFrom { get; set; }
    public Rating Rating { get; set; }
    public string RatingLong
    {
        get
        {
            return Rating switch
            {
                Rating.INAC => "Inactive",
                Rating.OBS => "Observer",
                Rating.S1 => "Student",
                Rating.S2 => "Student 2",
                Rating.S3 => "Student 3",
                Rating.C1 => "Controller",
                Rating.C2 => "Controller 2",
                Rating.C3 => "Senior Controller",
                Rating.I1 => "Instructor",
                Rating.I2 => "Instructor 2",
                Rating.I3 => "Senior Instructor",
                Rating.SUP => "Supervisor",
                Rating.ADM => "Administrator",
                _ => "None"
            };
        }
    }
    public AirportCert Minor { get; set; }
    public AirportCert Iad { get; set; }
    public AirportCert Dca { get; set; }
    public AirportCert Bwi { get; set; }
    public CenterCert Center { get; set; }
    public Access CanEvents { get; set; }
    public Access CanTraining { get; set; }
    public UserStatus Status { get; set; }
    public ICollection<Role>? Roles { get; set; }
    public DateTimeOffset Updated { get; set; }

    public User()
    {
        Minor = AirportCert.None;
        Iad = AirportCert.None;
        Dca = AirportCert.None;
        Bwi = AirportCert.None;
        Center = CenterCert.None;
        Status = UserStatus.Active;
        Updated = DateTimeOffset.UtcNow;
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

public enum Access
{
    Yes,
    No
}

public enum UserStatus
{
    Active,
    Inactive,
    Loa,
    Exempt,
    Removed
}