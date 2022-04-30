using System.Text.Json.Serialization;

namespace ZDC.Server.Services.Responses;

public class Ratings
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("rating")]
    public int Rating { get; set; }

    [JsonPropertyName("pilotrating")]
    public int PilotRating { get; set; }

    [JsonPropertyName("susp_date")]
    public string SuspendedDate { get; set; }

    [JsonPropertyName("reg_date")]
    public string RegistrationDate { get; set; }

    [JsonPropertyName("region")]
    public string Region { get; set; } = string.Empty;

    [JsonPropertyName("division")]
    public string Division { get; set; } = string.Empty;

    [JsonPropertyName("subdivision")]
    public string Subdivision { get; set; } = string.Empty;

    [JsonPropertyName("lastratingchange")]
    public string LastRatingChange { get; set; }
}
