using Microsoft.AspNetCore.Components;
using VoxDB.Components.Common.Services.Interfaces;
using VoxDB.Entities.Model;

namespace VoxDB.Components.Common.Components.ChatPanel;

public partial class ChatPanelComponent : IDisposable
{
    [Inject] public required IChatService ChatService { get; set; }
    [Parameter] public Guid SessionId { get; set; }
    [Inject] public required ILanguageService LanguageService { get; set; }

    [Parameter] public EventCallback OnUpdated { get; set; }

    private ChatSession? _session;
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

    protected override Task OnParametersSetAsync()
    {
        return ReloadAsync();
    }

    public async Task ReloadAsync()
    {
        _session = await ChatService.GetSessionAsync(SessionId);
        await InvokeAsync(StateHasChanged);
    }

    public Task RefreshAsync()
    {
        return ReloadAsync();
    }

    private RenderFragment RenderResultTable(string json)
    {
        return builder =>
        {
            try
            {
                var data = System.Text.Json.JsonSerializer.Deserialize<List<Employee>>(json);
                if (data is { Count: > 0 })
                {
                    string colId = "ID";
                    string colName = LanguageService.IsUa ? "ПІБ" : "Full Name";
                    string colPos = LanguageService.IsUa ? "Посада" : "Position";

                    builder.OpenElement(0, "table");
                    builder.AddAttribute(1, "class", "table");

                    builder.OpenElement(2, "thead");
                    builder.OpenElement(3, "tr");
                    builder.AddMarkupContent(4, $"<th>{colId}</th><th>{colName}</th><th>{colPos}</th>");
                    builder.CloseElement();
                    builder.CloseElement();

                    builder.OpenElement(5, "tbody");
                    foreach (var e in data)
                    {
                        builder.OpenElement(6, "tr");
                        builder.AddMarkupContent(7, $"<td>{e.Id}</td><td>{e.FullName}</td><td>{e.Position}</td>");
                        builder.CloseElement();
                    }
                    builder.CloseElement();

                    builder.CloseElement();
                }
            }
            catch
            {
            }
        };
    }
}
