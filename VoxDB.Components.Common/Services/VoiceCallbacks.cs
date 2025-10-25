using Microsoft.JSInterop;

namespace VoxDB.Components.Common.Services;

public class VoiceCallbacks
{
    [JSInvokable]
    public Task OnTranscript(string text, string? audioUrl)
    {
        return TranscriptReceived.Invoke(text, audioUrl);
    }

    public Func<string, string?, Task> TranscriptReceived { get; set; } = (_, _) => Task.CompletedTask;
}
