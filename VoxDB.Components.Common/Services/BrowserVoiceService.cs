using Microsoft.JSInterop;
using VoxDB.Components.Common.Services.Interfaces;

namespace VoxDB.Components.Common.Services;

public class BrowserVoiceService : IVoiceService, IDisposable
{
    private readonly IJSRuntime _jsRuntime;
    private readonly ILanguageService _languageService;

    public BrowserVoiceService(IJSRuntime jsRuntime, ILanguageService languageService)
    {
        _jsRuntime = jsRuntime;
        _languageService = languageService;
        _languageService.OnChanged += OnLangChanged;
    }

    public void Dispose()
    {
        _languageService.OnChanged -= OnLangChanged;
    }

    public Task<bool> IsSpeechSupportedAsync()
    {
        return _jsRuntime.InvokeAsync<bool>("vox.isSpeechAvailable").AsTask();
    }

    public Task StartListeningAsync(DotNetObjectReference<VoiceCallbacks> callbacks)
    {
        return _jsRuntime.InvokeVoidAsync("vox.startListening", callbacks).AsTask();
    }

    public Task StopListeningAsync()
    {
        return _jsRuntime.InvokeVoidAsync("vox.stopListening").AsTask();
    }

    private void OnLangChanged(string lang)
    {
        var mode = "auto";
        _ = _jsRuntime.InvokeVoidAsync("vox.setMode", mode);
    }
}
