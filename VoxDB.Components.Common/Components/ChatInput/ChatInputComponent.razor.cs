using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using VoxDB.Components.Common.Services;
using VoxDB.Components.Common.Services.Interfaces;

namespace VoxDB.Components.Common.Components.ChatInput;

public partial class ChatInputComponent : IDisposable
{
    [Inject] public required IChatService ChatService { get; set; }
    [Inject] public required IVoiceService VoiceService { get; set; }
    [Inject] public required ILanguageService LanguageService { get; set; }

    [Parameter] public Guid SessionId { get; set; }
    [Parameter] public EventCallback OnSend { get; set; }

    private string _text = "";
    private bool _speechAvailable;
    private bool _listening;
    private string? _lastTranscript;
    private string? _audioUrl;

    private VoiceCallbacks? _callbacksRef;
    private DotNetObjectReference<VoiceCallbacks>? _dotnetRef;

    private Action<string>? _handler;

    protected override void OnInitialized()
    {
        _handler = _ => StateHasChanged();
        LanguageService.OnChanged += _handler;
    }

    public void Dispose()
    {
        if (_handler != null)
            LanguageService.OnChanged -= _handler;
        _dotnetRef?.Dispose();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            _speechAvailable = await VoiceService.IsSpeechSupportedAsync();
            StateHasChanged();
        }
    }

    private async Task SendTextAsync()
    {
        if (string.IsNullOrWhiteSpace(_text))
            return;

        await ChatService.SendUserCommandAsync(SessionId, _text);

        _text = "";
        _lastTranscript = null;
        _audioUrl = null;
        _listening = false;

        await OnSend.InvokeAsync();
        StateHasChanged();
    }

    private async Task ToggleVoiceAsync()
    {
        if (!_listening)
        {
            _lastTranscript = null;
            _audioUrl = null;

            _callbacksRef = new VoiceCallbacks
            {
                TranscriptReceived = async (text, audioUrl) =>
                {
                    _lastTranscript = string.IsNullOrWhiteSpace(text) ? null : text;
                    _audioUrl = audioUrl;

                    if (!string.IsNullOrWhiteSpace(text))
                        _text = text;

                    _listening = false;
                    StateHasChanged();
                }
            };
            _dotnetRef?.Dispose();
            _dotnetRef = DotNetObjectReference.Create(_callbacksRef);

            await VoiceService.StartListeningAsync(_dotnetRef);
            _listening = true;
        }
        else
        {
            await VoiceService.StopListeningAsync();
            _listening = false;
        }

        StateHasChanged();
    }
}

