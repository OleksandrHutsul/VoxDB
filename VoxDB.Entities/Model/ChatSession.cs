namespace VoxDB.Entities.Model;

public class ChatSession
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid BrowserSessionId { get; set; }
    public string Title { get; set; } = "New chat";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public bool IsDeleted { get; set; }
    public List<ChatMessage> Messages { get; set; } = new();
}
