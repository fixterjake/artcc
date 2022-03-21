using System.Text.Json.Serialization;

namespace ZDC.Jobs.Models;

public class Status
{
    [JsonPropertyName("data")]
    public Data? Data { get; set; }
}

public class Data
{
    [JsonPropertyName("v3")]
    public IList<string>? V3 { get; set; }
}
