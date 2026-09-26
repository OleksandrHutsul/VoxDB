using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using VoxDB.Components.Common.DTOs;
using VoxDB.Components.Common.Enum;
using VoxDB.Components.Common.Extensions;
using VoxDB.Components.Common.Helper;
using VoxDB.Components.Common.Services.Interfaces;
using VoxDB.Entities.DbContext;
using VoxDB.Entities.Model;

namespace VoxDB.Components.Common.Services;

public class ChatService : IChatService
{
    private readonly VoxDbContext _voxDbContext;
    private readonly CommandInterpreter _commandInterpreter;
    private readonly ILanguageService _languageService;
    private readonly IBrowserSessionContext _browserSession;

    public ChatService(
        VoxDbContext voxDbContext,
        CommandInterpreter commandInterpreter,
        ILanguageService languageService,
        IBrowserSessionContext browserSession)
    {
        _voxDbContext = voxDbContext;
        _commandInterpreter = commandInterpreter;
        _languageService = languageService;
        _browserSession = browserSession;
    }

    public Task<List<ChatSession>> GetSessionsAsync(CancellationToken ct = default) =>
        OwnedChats().Where(s => !s.IsDeleted).OrderByDescending(s => s.CreatedAt).ToListAsync(ct);

    public async Task<ChatSession> CreateSessionAsync(string? title = null, CancellationToken ct = default)
    {
        var s = new ChatSession
        {
            BrowserSessionId = BrowserSessionId,
            Title = title ?? (_languageService.IsUa ? "Новий чат" : "New chat")
        };
        _voxDbContext.ChatSessions.Add(s);
        await _voxDbContext.SaveChangesAsync(ct);
        return s;
    }

    public async Task DeleteSessionAsync(Guid sessionId, CancellationToken ct = default)
    {
        var s = await OwnedChats().FirstOrDefaultAsync(x => x.Id == sessionId, ct);
        if (s is null) return;
        s.IsDeleted = true;
        await _voxDbContext.SaveChangesAsync(ct);
    }

    public Task<ChatSession?> GetSessionAsync(Guid id, CancellationToken ct = default)
    {
        var browserSessionId = BrowserSessionId;
        return OwnedChats()
            .Include(x => x.Messages.Where(m => m.BrowserSessionId == browserSessionId).OrderBy(m => m.CreatedAt))
            .FirstOrDefaultAsync(x => x.Id == id, ct);
    }

    public async Task<CommandResult> SendUserCommandAsync(Guid sessionId, string userText, string? audioUrl = null, CancellationToken ct = default)
    {
        var exists = await OwnedChats().AnyAsync(s => s.Id == sessionId && !s.IsDeleted, ct);
        if (!exists)
        {
            var created = await CreateSessionAsync(ct: ct);
            sessionId = created.Id;
        }

        var browserSessionId = BrowserSessionId;
        var userMsg = new ChatMessage
        {
            BrowserSessionId = browserSessionId,
            ChatSessionId = sessionId,
            Role = "user",
            Text = userText,
            AudioUrl = audioUrl
        };
        _voxDbContext.ChatMessages.Add(userMsg);

        var parsed = _commandInterpreter.Parse(userText);
        var result = await ExecuteAsync(parsed, ct);

        var systemMsg = new ChatMessage
        {
            BrowserSessionId = browserSessionId,
            ChatSessionId = sessionId,
            Role = "system",
            Text = result.Message,
            JsonResult = result.Data is null ? null : JsonSerializer.Serialize(result.Data)
        };
        _voxDbContext.ChatMessages.Add(systemMsg);

        var session = await OwnedChats().FirstAsync(s => s.Id == sessionId, ct);
        var defaultTitle = _languageService.IsUa ? "Новий чат" : "New chat";
        if (session.Title == defaultTitle || string.IsNullOrWhiteSpace(session.Title))
            session.Title = userText.Length > 60 ? userText[..60] + "…" : userText;

        await _voxDbContext.SaveChangesAsync(ct);
        return result;
    }

    private async Task<CommandResult> ExecuteAsync(ParsedCommand cmd, CancellationToken ct)
    {
        switch (cmd.Kind)
        {
            case CommandKind.SelectAllEmployees:
                {
                    var list = await OwnedEmployees().OrderBy(e => e.Id).ToListAsync(ct);
                    return new CommandResult { Success = true, Message = CommandHelper.FoundEmployees(_languageService, list.Count), Data = list };
                }
            case CommandKind.AddEmployee:
                {
                    if (!cmd.Args.TryGetValue("name", out var name) || string.IsNullOrWhiteSpace(name))
                        return Fail(CommandHelper.MissingEmployeeName(_languageService));
                    _voxDbContext.Employees.Add(new Employee
                    {
                        BrowserSessionId = BrowserSessionId,
                        FullName = name,
                        Position = _languageService.IsUa ? "Невідомо" : "Unknown"
                    });
                    await _voxDbContext.SaveChangesAsync(ct);
                    return Ok(CommandHelper.AddedEmployee(_languageService, name));
                }
            case CommandKind.UpdateEmployeePosition:
                {
                    if (!cmd.Args.TryGetValue("id", out var sId) || !int.TryParse(sId, out var id) ||
                        !cmd.Args.TryGetValue("pos", out var pos) || string.IsNullOrWhiteSpace(pos))
                        return Fail(CommandHelper.NeedIdAndPosition(_languageService));

                    var emp = await OwnedEmployees().FirstOrDefaultAsync(e => e.Id == id, ct);
                    if (emp is null) return Fail(CommandHelper.EmployeeNotExists(_languageService, id));

                    emp.Position = pos;
                    await _voxDbContext.SaveChangesAsync(ct);
                    return Ok(CommandHelper.UpdatedEmployeePosition(_languageService, id, pos));
                }
            case CommandKind.DeleteEmployeeById:
                {
                    if (!cmd.Args.TryGetValue("id", out var sId) || !int.TryParse(sId, out var id))
                        return Fail(CommandHelper.InvalidId(_languageService));

                    var toDel = await OwnedEmployees().FirstOrDefaultAsync(e => e.Id == id, ct);
                    if (toDel is null) return Fail(CommandHelper.EmployeeNotExists(_languageService, id));

                    _voxDbContext.Employees.Remove(toDel);
                    await _voxDbContext.SaveChangesAsync(ct);
                    return Ok(CommandHelper.DeletedEmployee(_languageService, id));
                }
            default:
                return Fail(CommandHelper.UnknownCommand(_languageService));
        }

        static CommandResult Ok(string m) => new() { Success = true, Message = m };
        static CommandResult Fail(string m) => new() { Success = false, Message = m };
    }

    private Guid BrowserSessionId
    {
        get
        {
            if (!_browserSession.IsReady || _browserSession.SessionId == Guid.Empty)
                throw new InvalidOperationException("Browser session is not initialized.");

            return _browserSession.SessionId;
        }
    }

    private IQueryable<Employee> OwnedEmployees()
    {
        var browserSessionId = BrowserSessionId;
        return _voxDbContext.Employees.Where(e => e.BrowserSessionId == browserSessionId);
    }

    private IQueryable<ChatSession> OwnedChats()
    {
        var browserSessionId = BrowserSessionId;
        return _voxDbContext.ChatSessions.Where(s => s.BrowserSessionId == browserSessionId);
    }
}
