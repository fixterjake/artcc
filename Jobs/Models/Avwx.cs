using System.Text.Json.Serialization;

namespace ZDC.Jobs.Models;

public class Weather
{
    [JsonPropertyName("altimeter")]
    public Altimeter? Altimeter { get; set; }

    [JsonPropertyName("flight_rules")]
    public string? FlightRules { get; set; }

    [JsonPropertyName("wind_direction")]
    public WindDirection? WindDirection { get; set; }

    [JsonPropertyName("wind_speed")]
    public WindSpeed? WindSpeed { get; set; }

    [JsonPropertyName("wind_gust")]
    public WindGust? WindGust { get; set; }

    [JsonPropertyName("sanitized")]
    public string? MetarRaw { get; set; }
}

public class Altimeter
{
    [JsonPropertyName("value")]
    public double Value { get; set; }
}

public class WindDirection
{
    [JsonPropertyName("value")]
    public int Value { get; set; }
}

public class WindSpeed
{
    [JsonPropertyName("value")]
    public int Value { get; set; }
}

public class WindGust
{
    [JsonPropertyName("value")]
    public int Value { get; set; }
}
