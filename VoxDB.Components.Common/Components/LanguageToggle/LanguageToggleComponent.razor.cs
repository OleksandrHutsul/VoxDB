using Microsoft.AspNetCore.Components;
using VoxDB.Components.Common.Services.Interfaces;

namespace VoxDB.Components.Common.Components.LanguageToggle;

public partial class LanguageToggleComponent
{
    [Inject] public required ILanguageService LanguageService { get; set; }

    private void SetLang(string l)
    {
        LanguageService.Set(l);
        StateHasChanged();
    }
}
