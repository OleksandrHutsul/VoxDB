using Microsoft.AspNetCore.Components;
using VoxDB.Components.Common.Services.Interfaces;

namespace VoxDB.Components.Common.Components.CommandList;

public partial class CommandListComponent: IDisposable
{
    [Inject] public required ILanguageService LanguageService { get; set; }

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
}
