using VoxDB.Components.Common.Enum;

namespace VoxDB.Components.Common.Extensions;

public record ParsedCommand(CommandKind Kind, Dictionary<string, string> Args);
