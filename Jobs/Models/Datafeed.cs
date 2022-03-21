using System.Text.Json.Serialization;

namespace ZDC.Jobs.Models;

public class Datafeed
{
    [JsonPropertyName("pilots")]
    public IList<Pilot>? Pilots { get; set; }

    [JsonPropertyName("controllers")]
    public IList<Controller>? Controllers { get; set; }
}

public class Pilot
{
    [JsonPropertyName("cid")]
    public int Cid { get; set; }

    [JsonPropertyName("latitude")]
    public double Latitude { get; set; }

    [JsonPropertyName("longitude")]
    public double Longitude { get; set; }

    [JsonPropertyName("altitude")]
    public int Altitude { get; set; }

    [JsonPropertyName("groundspeed")]
    public int Groundspeed { get; set; }

    [JsonPropertyName("heading")]
    public int Heading { get; set; }

    [JsonPropertyName("flight_plan")]
    public Flightplan? Flightplan { get; set; }
}

public class Flightplan
{
    [JsonPropertyName("aircraft_faa")]
    public string Aircraft { get; set; } = string.Empty;

    [JsonPropertyName("departure")]
    public string Departure { get; set; } = string.Empty;

    [JsonPropertyName("arrival")]
    public string Arrival { get; set; } = string.Empty;
}

public class Controller
{
    [JsonPropertyName("cid")]
    public int Cid { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("callsign")]
    public string Callsign { get; set; } = string.Empty;

    [JsonPropertyName("frequency")]
    public string Frequency { get; set; } = string.Empty;

    [JsonPropertyName("logon_time")]
    public DateTimeOffset Login { get; set; }
}
