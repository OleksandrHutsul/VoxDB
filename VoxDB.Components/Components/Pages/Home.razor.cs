using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using VoxDB.Components.Common.Components.ChatInput;
using VoxDB.Components.Common.Components.ChatPanel;
using VoxDB.Components.Common.Services.Interfaces;

namespace VoxDB.Components.Components.Pages;

public partial class Home
{
    [Inject] public required IChatService ChatService { get; set; }
    [Inject] public required IJSRuntime JS { get; set; }
    [Inject] public required IBrowserSessionContext BrowserSessionContext { get; set; }
    [Inject] public required IBrowserSessionService BrowserSessionService { get; set; }

    private Guid _sessionId;
    private bool _languageReady;
    private bool _browserReady;
    private ChatPanelComponent? _chatPane;
    private ChatInputComponent? _chatInput;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!firstRender) return;

        await LanguageService.RestoreAsync();
        _languageReady = true;
        StateHasChanged();

        var browserSessionId = await ResolveBrowserSessionIdAsync();
        BrowserSessionContext.Set(browserSessionId);
        await BrowserSessionService.EnsureAsync(browserSessionId);

        var sessions = await ChatService.GetSessionsAsync();
        if (sessions.Count == 0)
            _sessionId = (await ChatService.CreateSessionAsync()).Id;
        else
            _sessionId = sessions.First().Id;

        _browserReady = true;
        StateHasChanged();
    }

    private async Task<Guid> ResolveBrowserSessionIdAsync()
    {
        var raw = await JS.InvokeAsync<string>("vox.getOrCreateSessionId");
        if (Guid.TryParse(raw, out var id) && id != Guid.Empty)
            return id;

        id = Guid.NewGuid();
        await JS.InvokeVoidAsync("vox.setSessionId", id.ToString("D"));
        return id;
    }

    private Task OpenChat(Guid id)
    {
        _sessionId = id;
        StateHasChanged();
        return Task.CompletedTask;
    }

    private async Task CreateChat()
    {
        var s = await ChatService.CreateSessionAsync();
        _sessionId = s.Id;
    }

    private async Task DeleteChat(Guid id)
    {
        await ChatService.DeleteSessionAsync(id);

        if (_sessionId == id)
        {
            var sessions = await ChatService.GetSessionsAsync();
            if (sessions.Count == 0)
                _sessionId = (await ChatService.CreateSessionAsync()).Id;
            else
                _sessionId = sessions.First().Id;
        }

        StateHasChanged();
    }

    private async Task UseExample(string command)
    {
        if (_chatInput is not null)
            await _chatInput.SetCommandAsync(command);
    }

    private async Task HandleSend()
    {
        if (_chatPane is not null)
            await _chatPane.ReloadAsync();
    }
}
