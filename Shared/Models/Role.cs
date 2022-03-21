using System.Text.Json.Serialization;

namespace ZDC.Shared.Models;

public class Role
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string NameLong { get; set; } = string.Empty;
    public int Priority { get; set; }
    [JsonIgnore] public ICollection<User>? Users { get; set; }
}