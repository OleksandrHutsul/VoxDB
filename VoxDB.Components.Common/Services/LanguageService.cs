using VoxDB.Components.Common.Services.Interfaces;

namespace VoxDB.Components.Common.Services;

public class LanguageService : ILanguageService
{
    public string Current { get; private set; } = "ua";
    public bool IsUa => Current == "ua";
    public bool IsEn => Current == "en";

    public event Action<string>? OnChanged;

    public void Set(string lang)
    {
        if (lang != "ua" && lang != "en") lang = "ua";
        if (Current == lang) return;
        Current = lang;
        OnChanged?.Invoke(Current);
    }
}
