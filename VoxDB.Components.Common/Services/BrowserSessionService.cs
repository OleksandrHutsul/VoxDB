using Microsoft.EntityFrameworkCore;
using VoxDB.Components.Common.Services.Interfaces;
using VoxDB.Entities.DbContext;
using VoxDB.Entities.Model;

namespace VoxDB.Components.Common.Services;

public class BrowserSessionService : IBrowserSessionService
{
    public static readonly TimeSpan Retention = TimeSpan.FromDays(30);

    private readonly VoxDbContext _db;

    public BrowserSessionService(VoxDbContext db)
    {
        _db = db;
    }

    public async Task EnsureAsync(Guid sessionId, CancellationToken ct = default)
    {
        if (sessionId == Guid.Empty)
            throw new ArgumentException("Session id is required.", nameof(sessionId));

        var now = DateTime.UtcNow;
        var existing = await _db.BrowserSessions.FirstOrDefaultAsync(s => s.Id == sessionId, ct);
        if (existing is null)
        {
            _db.BrowserSessions.Add(new BrowserSession
            {
                Id = sessionId,
                CreatedAt = now,
                LastSeenAt = now
            });
            _db.Employees.AddRange(DemoEmployees(sessionId));

            try
            {
                await _db.SaveChangesAsync(ct);
            }
            catch (DbUpdateException)
            {
                _db.ChangeTracker.Clear();
                existing = await _db.BrowserSessions.FirstOrDefaultAsync(s => s.Id == sessionId, ct);
                if (existing is null)
                    throw;

                existing.LastSeenAt = now;
                await _db.SaveChangesAsync(ct);
            }
        }
        else
        {
            existing.LastSeenAt = now;
            await _db.SaveChangesAsync(ct);
        }
    }

    public async Task PurgeExpiredAsync(CancellationToken ct = default)
    {
        var cutoff = DateTime.UtcNow.Subtract(Retention);
        var expiredIds = await _db.BrowserSessions
            .Where(s => s.LastSeenAt < cutoff)
            .Select(s => s.Id)
            .ToListAsync(ct);

        if (expiredIds.Count == 0)
            return;

        await _db.ChatMessages.Where(m => expiredIds.Contains(m.BrowserSessionId)).ExecuteDeleteAsync(ct);
        await _db.ChatSessions.Where(s => expiredIds.Contains(s.BrowserSessionId)).ExecuteDeleteAsync(ct);
        await _db.Employees.Where(e => expiredIds.Contains(e.BrowserSessionId)).ExecuteDeleteAsync(ct);
        await _db.BrowserSessions.Where(s => expiredIds.Contains(s.Id)).ExecuteDeleteAsync(ct);
    }

    private static Employee[] DemoEmployees(Guid sessionId) =>
    [
        new Employee { BrowserSessionId = sessionId, FullName = "Ivan Ivanov", Position = "Engineer" },
        new Employee { BrowserSessionId = sessionId, FullName = "Alex Baena", Position = "Analyst" }
    ];
}
