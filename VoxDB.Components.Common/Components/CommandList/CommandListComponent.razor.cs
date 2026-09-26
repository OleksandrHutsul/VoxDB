using Microsoft.AspNetCore.Components;
using VoxDB.Components.Common.Services.Interfaces;

namespace VoxDB.Components.Common.Components.CommandList;

public partial class CommandListComponent: IDisposable
{
    private static readonly (string Kind, string Text)[] UkrainianExamples =
    [
        ("select", "Покажи всіх працівників"),
        ("add", "Додай працівника Іван Іванов"),
        ("update", "Онови посаду працівника з ID 3 на менеджер"),
        ("delete", "Видали працівника з ID 5")
    ];

    private static readonly (string Kind, string Text)[] EnglishExamples =
    [
        ("select", "Show all employees"),
        ("add", "Add employee John Smith"),
        ("update", "Update employee with ID 3 to manager"),
        ("delete", "Delete employee with ID 5")
    ];

    [Inject] public required ILanguageService LanguageService { get; set; }

    [Parameter] public EventCallback<string> OnExampleSelected { get; set; }

    private Action<string>? _handler;

    private IReadOnlyList<(string Kind, string Text)> Examples => LanguageService.IsUa ? UkrainianExamples : EnglishExamples;

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

    private Task SelectAsync(string command)
    {
        return OnExampleSelected.InvokeAsync(command);
    }
}
