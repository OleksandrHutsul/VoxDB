namespace VoxDB.Components.Common.Services.Interfaces;

public interface ILanguageService
{
    string Current { get; }
    bool IsUa { get; }
    bool IsEn { get; }
    void Set(string lang);
    event Action<string>? OnChanged;
}

