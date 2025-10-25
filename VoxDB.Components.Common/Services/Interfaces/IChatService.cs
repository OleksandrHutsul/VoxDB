using VoxDB.Components.Common.DTOs;
using VoxDB.Entities.Model;

namespace VoxDB.Components.Common.Services.Interfaces;

public interface IChatService
{
    Task<List<ChatSession>> GetSessionsAsync(CancellationToken ct = default);
    Task<ChatSession> CreateSessionAsync(string? title = null, CancellationToken ct = default);
    Task DeleteSessionAsync(Guid sessionId, CancellationToken ct = default);
    Task<ChatSession?> GetSessionAsync(Guid id, CancellationToken ct = default);
    Task<CommandResult> SendUserCommandAsync(Guid sessionId, string userText, string? audioUrl = null, CancellationToken ct = default);
}
