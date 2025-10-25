using Microsoft.EntityFrameworkCore;
using VoxDB.Entities.Model;

namespace VoxDB.Entities.DbContext;

public class VoxDbContext : Microsoft.EntityFrameworkCore.DbContext
{
    public DbSet<Employee> Employees => Set<Employee>();
    public DbSet<ChatSession> ChatSessions => Set<ChatSession>();
    public DbSet<ChatMessage> ChatMessages => Set<ChatMessage>();

    public VoxDbContext(DbContextOptions<VoxDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder b)
    {
        b.Entity<Employee>().HasKey(x => x.Id);
        b.Entity<Employee>().Property(x => x.FullName).IsRequired();
        b.Entity<Employee>().Property(x => x.Position).HasMaxLength(128);

        b.Entity<ChatSession>().HasKey(x => x.Id);
        b.Entity<ChatSession>().Property(x => x.Title).HasMaxLength(200);
        b.Entity<ChatSession>().HasMany(x => x.Messages)
            .WithOne().HasForeignKey(x => x.ChatSessionId)
            .OnDelete(DeleteBehavior.Cascade);

        b.Entity<ChatMessage>().HasKey(x => x.Id);
        b.Entity<ChatMessage>().Property(x => x.Role).HasMaxLength(20);

        b.Entity<Employee>().HasData(
            new Employee { Id = 1, FullName = "Ivan Ivanov", Position = "Engineer" },
            new Employee { Id = 2, FullName = "Alex Baena", Position = "Analyst" }
        );
    }
}
