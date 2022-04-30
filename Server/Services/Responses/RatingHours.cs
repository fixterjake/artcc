using System.Text.Json.Serialization;

namespace ZDC.Server.Services.Responses;

public class RatingHours
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("atc")]
    public double Atc { get; set; }

    [JsonPropertyName("pilot")]
    public double Pilot { get; set; }

    [JsonPropertyName("s1")]
    public double S1 { get; set; }

    [JsonPropertyName("s2")]
    public double S2 { get; set; }

    [JsonPropertyName("s3")]
    public double S3 { get; set; }

    [JsonPropertyName("c1")]
    public double C1 { get; set; }

    [JsonPropertyName("c2")]
    public double C2 { get; set; }

    [JsonPropertyName("c3")]
    public double C3 { get; set; }

    [JsonPropertyName("i1")]
    public double I1 { get; set; }

    [JsonPropertyName("i2")]
    public double I2 { get; set; }

    [JsonPropertyName("i3")]
    public double I3 { get; set; }

    [JsonPropertyName("sup")]
    public double Sup { get; set; }

    [JsonPropertyName("adm")]
    public double Adm { get; set; }
}
