using Microsoft.AspNetCore.Components;
using VoxDB.Components.Common.Components.ChatPanel;
using VoxDB.Components.Common.Services.Interfaces;

namespace VoxDB.Components.Components.Pages;

public partial class Home
{
    [Inject] public required IChatService ChatService { get; set; }

    private Guid _sessionId;
    private ChatPanelComponent? _chatPane;

    protected override async Task OnInitializedAsync()
    {
        var sessions = await ChatService.GetSessionsAsync();
        if (sessions.Count == 0)
            _sessionId = (await ChatService.CreateSessionAsync()).Id;
        else
            _sessionId = sessions.First().Id;
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

    private async Task HandleSend()
    {
        if (_chatPane is not null)
            await _chatPane.ReloadAsync();
    }
}
