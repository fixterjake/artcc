namespace ZDC.Shared.Models;

public class Airport
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Icao { get; set; } = string.Empty;
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public int OutboundFlights { get; set; }
    public int InboundFlights { get; set; }
    public string Conditions { get; set; } = string.Empty;
    public string Winds { get; set; } = string.Empty;
    public string Altimeter { get; set; } = string.Empty;
    public string Metar { get; set; } = string.Empty;
    public DateTimeOffset Updated { get; set; }

    public Airport()
    {
        Updated = DateTimeOffset.UtcNow;
    }
}