using ZDC.Shared.Models;

namespace ZDC.Shared.Extensions;

public static class EnumExtensions
{
    public static string ToString(this Rating rating)
    {
        return rating switch
        {
            Rating.INAC => "Inactive",
            Rating.SUS => "Suspended",
            Rating.OBS => "Observer",
            Rating.S1 => "Student",
            Rating.S2 => "Student 2",
            Rating.S3 => "Student 3",
            Rating.C1 => "Controller",
            Rating.C2 => "Controller 2",
            Rating.C3 => "Controller 3",
            Rating.I1 => "Instructor",
            Rating.I2 => "Instructor 2",
            Rating.I3 => "Instructor 3",
            Rating.SUP => "Supervisor",
            Rating.ADM => "Administrator",
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    public static string ToString(this AirportCert cert)
    {
        return cert switch
        {
            AirportCert.None => "None",
            AirportCert.SoloGround => "Solo Ground",
            AirportCert.Ground => "Ground",
            AirportCert.SoloTower => "Solo Tower",
            AirportCert.Tower => "Tower",
            AirportCert.SoloApproach => "Solo Approach",
            AirportCert.Approach => "Approach",
            _ => throw new ArgumentOutOfRangeException(nameof(cert), cert, null)
        };
    }
}