using Microsoft.JSInterop;

namespace VoxDB.Components.Common.Services.Interfaces;

public interface IVoiceService
{
    Task<bool> IsSpeechSupportedAsync();
    Task StartListeningAsync(DotNetObjectReference<VoiceCallbacks> callbacks);
    Task StopListeningAsync();
}
