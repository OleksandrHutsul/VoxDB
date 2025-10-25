using System.Text.RegularExpressions;
using VoxDB.Components.Common.Enum;
using VoxDB.Components.Common.Extensions;
using VoxDB.Components.Common.Services.Interfaces;

namespace VoxDB.Components.Common.Services;

public class CommandInterpreter
{
    private readonly ILanguageService _languageService;

    public CommandInterpreter(ILanguageService languageService)
    {
        _languageService = languageService;
    }

    private static readonly Regex RxSelectAllUA = new(
        @"^\s*(виб(ери|рати)|покажи|показати)\s+(всіх|усіх)\s+працівників\s*$",
        RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);

    private static readonly Regex RxAddUA = new(
        @"^\s*додай(те)?\s+працівника\s+(?<name>.+?)\s*$",
        RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);

    private static readonly Regex RxUpdateByIdUA = new(
        @"^\s*онови(ть)?\s+(посаду\s+)?працівника\s+(з\s+)?(id|ідентифікатором)\s+(?<id>\d+)\s+на\s+(?<pos>.+?)\s*$",
        RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);

    private static readonly Regex RxDeleteByIdUA = new(
        @"^\s*видали(ть)?\s+працівника\s+(з\s+)?(id|ідентифікатором)\s+(?<id>\d+)\s*$",
        RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);

    private static readonly Regex RxSelectAllEN = new(
        @"^\s*(show|list)\s+(all\s+)?employees\s*$",
        RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);

    private static readonly Regex RxAddEN = new(
        @"^\s*(add|create)\s+employee\s+(?<name>.+?)\s*$",
        RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);

    private static readonly Regex RxUpdateByIdEN = new(
        @"^\s*(update|set)\s+((the\s+)?(position\s+of\s+)?)?employee\s+(with\s+)?(id|identifier)\s+(?<id>\d+)\s+(to|as)\s+(?<pos>.+?)\s*$",
        RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);

    private static readonly Regex RxDeleteByIdEN = new(
        @"^\s*(delete|remove)\s+employee\s+(with\s+)?(id|identifier)\s+(?<id>\d+)\s*$",
        RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);

    public ParsedCommand Parse(string input)
    {
        input = input.Trim();

        if (_languageService.IsUa)
        {
            if (RxSelectAllUA.IsMatch(input)) return new(CommandKind.SelectAllEmployees, new());
            var add = RxAddUA.Match(input); if (add.Success) return new(CommandKind.AddEmployee, new() { ["name"] = add.Groups["name"].Value.Trim() });
            var upd = RxUpdateByIdUA.Match(input); if (upd.Success) return new(CommandKind.UpdateEmployeePosition, new() { ["id"] = upd.Groups["id"].Value, ["pos"] = upd.Groups["pos"].Value.Trim() });
            var del = RxDeleteByIdUA.Match(input); if (del.Success) return new(CommandKind.DeleteEmployeeById, new() { ["id"] = del.Groups["id"].Value });
        }
        else 
        {
            if (RxSelectAllEN.IsMatch(input)) return new(CommandKind.SelectAllEmployees, new());
            var add = RxAddEN.Match(input); if (add.Success) return new(CommandKind.AddEmployee, new() { ["name"] = add.Groups["name"].Value.Trim() });
            var upd = RxUpdateByIdEN.Match(input); if (upd.Success) return new(CommandKind.UpdateEmployeePosition, new() { ["id"] = upd.Groups["id"].Value, ["pos"] = upd.Groups["pos"].Value.Trim() });
            var del = RxDeleteByIdEN.Match(input); if (del.Success) return new(CommandKind.DeleteEmployeeById, new() { ["id"] = del.Groups["id"].Value });
        }

        return new(CommandKind.Unknown, new());
    }
}
