using Microsoft.EntityFrameworkCore;
using VoxDB.Entities.Model;

namespace VoxDB.Entities.DbContext;

public class VoxDbContext : Microsoft.EntityFrameworkCore.DbContext
{
    public DbSet<BrowserSession> BrowserSessions => Set<BrowserSession>();
    public DbSet<Employee> Employees => Set<Employee>();
    public DbSet<ChatSession> ChatSessions => Set<ChatSession>();
    public DbSet<ChatMessage> ChatMessages => Set<ChatMessage>();

    public VoxDbContext(DbContextOptions<VoxDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder b)
    {
        b.Entity<BrowserSession>().HasKey(x => x.Id);

        b.Entity<Employee>().HasKey(x => x.Id);
        b.Entity<Employee>().Property(x => x.FullName).IsRequired();
        b.Entity<Employee>().Property(x => x.Position).HasMaxLength(128);
        b.Entity<Employee>().HasIndex(x => x.BrowserSessionId);
        b.Entity<Employee>().HasOne<BrowserSession>()
            .WithMany()
            .HasForeignKey(x => x.BrowserSessionId)
            .OnDelete(DeleteBehavior.Cascade);

        b.Entity<ChatSession>().HasKey(x => x.Id);
        b.Entity<ChatSession>().Property(x => x.Title).HasMaxLength(200);
        b.Entity<ChatSession>().HasIndex(x => x.BrowserSessionId);
        b.Entity<ChatSession>().HasOne<BrowserSession>()
            .WithMany()
            .HasForeignKey(x => x.BrowserSessionId)
            .OnDelete(DeleteBehavior.Cascade);
        b.Entity<ChatSession>().HasMany(x => x.Messages)
            .WithOne().HasForeignKey(x => x.ChatSessionId)
            .OnDelete(DeleteBehavior.Cascade);

        b.Entity<ChatMessage>().HasKey(x => x.Id);
        b.Entity<ChatMessage>().Property(x => x.Role).HasMaxLength(20);
        b.Entity<ChatMessage>().HasIndex(x => x.BrowserSessionId);
        b.Entity<ChatMessage>().HasOne<BrowserSession>()
            .WithMany()
            .HasForeignKey(x => x.BrowserSessionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
