namespace VoxDB.Entities.Model;

public class ChatMessage
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid ChatSessionId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public string Role { get; set; } = "user";
    public string? Text { get; set; }
    public string? AudioUrl { get; set; }
    public string? JsonResult { get; set; }
}
