using VoxDB.Components.Common.Services.Interfaces;

namespace VoxDB.Components.Common.Services;

public sealed class BrowserSessionContext : IBrowserSessionContext
{
    public Guid SessionId { get; private set; }
    public bool IsReady { get; private set; }

    public void Set(Guid sessionId)
    {
        if (sessionId == Guid.Empty)
            throw new ArgumentException("Session id is required.", nameof(sessionId));

        SessionId = sessionId;
        IsReady = true;
    }
}
