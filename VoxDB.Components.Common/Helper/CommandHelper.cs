using VoxDB.Components.Common.Services.Interfaces;

namespace VoxDB.Components.Common.Helper;

public class CommandHelper
{
    public static string FoundEmployees(ILanguageService L, int n)
            => L.IsUa ? $"Знайдено {n} працівників." : $"Found {n} employees.";

    public static string AddedEmployee(ILanguageService L, string name)
        => L.IsUa ? $"Додано працівника “{name}”." : $"Added employee “{name}”.";

    public static string MissingEmployeeName(ILanguageService L)
        => L.IsUa ? "Не вказано ім’я працівника." : "Employee name is missing.";

    public static string NeedIdAndPosition(ILanguageService L)
        => L.IsUa
            ? "Потрібно: ID та нова посада (напр. “онови посаду працівника з id 3 на менеджер”)."
            : "Required: ID and new position (e.g. “update employee with id 3 to manager”).";

    public static string EmployeeNotExists(ILanguageService L, int id)
        => L.IsUa
            ? $"Працівника з ID={id} не існує. Перегляньте список через “покажи всіх працівників”."
            : $"Employee with ID={id} does not exist. Use “show all employees” to review the list.";

    public static string UpdatedEmployeePosition(ILanguageService L, int id, string pos)
        => L.IsUa
            ? $"Оновлено посаду працівника ID={id} на “{pos}”."
            : $"Updated position of employee ID={id} to “{pos}”.";

    public static string InvalidId(ILanguageService L)
        => L.IsUa ? "Некоректний ідентифікатор." : "Invalid identifier.";

    public static string DeletedEmployee(ILanguageService L, int id)
        => L.IsUa ? $"Видалено працівника ID={id}." : $"Deleted employee ID={id}.";

    public static string UnknownCommand(ILanguageService L)
        => L.IsUa ? "Невідома команда. Відкрийте список команд." : "Unknown command. Open the commands list.";
}
