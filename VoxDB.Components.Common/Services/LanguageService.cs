using Microsoft.JSInterop;
using VoxDB.Components.Common.Services.Interfaces;

namespace VoxDB.Components.Common.Services;

public class LanguageService : ILanguageService
{
    private readonly IJSRuntime _js;

    public LanguageService(IJSRuntime js)
    {
        _js = js;
    }

    public string Current { get; private set; } = "ua";
    public bool IsUa => Current == "ua";
    public bool IsEn => Current == "en";

    public event Action<string>? OnChanged;

    public async Task RestoreAsync()
    {
        string? stored;
        try
        {
            stored = await _js.InvokeAsync<string?>("vox.getLanguage");
        }
        catch (Exception ex) when (ex is InvalidOperationException or JSException)
        {
            return;
        }

        if (stored == "ua" || stored == "en")
            Set(stored);
    }

    public void Set(string lang)
    {
        if (lang != "ua" && lang != "en") lang = "ua";
        var changed = Current != lang;
        Current = lang;
        _ = PersistAsync(Current);
        if (changed)
            OnChanged?.Invoke(Current);
    }

    private async Task PersistAsync(string lang)
    {
        try
        {
            await _js.InvokeVoidAsync("vox.setLanguage", lang);
        }
        catch (Exception ex) when (ex is InvalidOperationException or JSException)
        {
        }
    }
}
