using System.Text.Json.Serialization;
using ZDC.Shared.Models;

namespace ZDC.Jobs.Models;

public class Roster
{
    [JsonPropertyName("data")]
    public IList<RosterEntry>? Data { get; set; }
}

public class RosterEntry
{
    [JsonPropertyName("cid")]
    public int Cid { get; set; }

    [JsonPropertyName("fname")]
    public string FirstName { get; set; } = string.Empty;

    [JsonPropertyName("lname")]
    public string LastName { get; set; } = string.Empty;

    [JsonPropertyName("email")]
    public string Email { get; set; } = string.Empty;

    [JsonPropertyName("facility")]
    public string Facility { get; set; } = string.Empty;

    [JsonPropertyName("rating")]
    public Rating Rating { get; set; }

    [JsonPropertyName("facility_join")]
    public DateTimeOffset Joined { get; set; }

    [JsonPropertyName("membership")]
    public string Membership { get; set; } = string.Empty;
}
