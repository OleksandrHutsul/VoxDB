using Microsoft.AspNetCore.Components;
using VoxDB.Components.Common.Services.Interfaces;
using VoxDB.Entities.Model;

namespace VoxDB.Components.Common.Components.ChatHistory;

public partial class ChatHistoryComponent
{
    [Inject] public required IChatService ChatService { get; set; }
    [Inject] public required ILanguageService LanguageService { get; set; }
    
    [Parameter] public Guid SelectedId { get; set; }
    [Parameter] public EventCallback<Guid> OnOpen { get; set; }
    [Parameter] public EventCallback OnCreate { get; set; }
    [Parameter] public EventCallback<Guid> OnDelete { get; set; }

    private List<ChatSession>? _sessions;
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
    }

    protected override async Task OnParametersSetAsync()
    {
        _sessions = await ChatService.GetSessionsAsync();
    }

    private Task Create()
    {
        return OnCreate.InvokeAsync();
    }

    private Task Delete(Guid id)
    {
        return OnDelete.InvokeAsync(id);
    }

    private Task Open(Guid id)
    {
        return OnOpen.InvokeAsync(id);
    }
}
