namespace VoxDB.Components.Common.Services.Interfaces;

public interface IBrowserSessionService
{
    Task EnsureAsync(Guid sessionId, CancellationToken ct = default);
    Task PurgeExpiredAsync(CancellationToken ct = default);
}
