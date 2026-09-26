using System.Text.Json.Serialization;

namespace VoxDB.Entities.Model;

public class Employee
{
    public int Id { get; set; }
    [JsonIgnore]
    public Guid BrowserSessionId { get; set; }
    public string FullName { get; set; } = "";
    public string? Position { get; set; }
}
