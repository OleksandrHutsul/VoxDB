namespace VoxDB.Components.Common.Services.Interfaces;

public interface IBrowserSessionContext
{
    Guid SessionId { get; }
    bool IsReady { get; }
    void Set(Guid sessionId);
}
