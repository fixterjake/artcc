using System.Text.Json.Serialization;

namespace ZDC.Shared.Models;

public class Role
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string NameLong { get; set; }
    public int Priority { get; set; }
    [JsonIgnore] public ICollection<User> Users { get; set; }
}