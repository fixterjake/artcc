namespace ZDC.Shared.Models;

public class Airport
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Icao { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public int OutboundFlights { get; set; }
    public int InboundFlights { get; set; }
    public string Conditions { get; set; }
    public string Winds { get; set; }
    public string Altimeter { get; set; }
    public string Metar { get; set; }
    public DateTime Updated { get; set; }

    public Airport()
    {
        Updated = DateTime.UtcNow;
    }
}