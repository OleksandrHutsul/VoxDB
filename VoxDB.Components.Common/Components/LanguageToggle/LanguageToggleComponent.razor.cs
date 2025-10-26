using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using VoxDB.Components.Common.Services.Interfaces;

namespace VoxDB.Components.Common.Components.LanguageToggle;

public partial class LanguageToggleComponent
{
    [Inject] public required ILanguageService LanguageService { get; set; }
    [Inject] public required IJSRuntime JSRuntime { get; set; }

    private async Task SetLangAsync(string l)
    {
        LanguageService.Set(l);
        await JSRuntime.InvokeVoidAsync("vox.setMode", LanguageService.IsUa ? "ua" : "en");
        StateHasChanged();
    }
}
