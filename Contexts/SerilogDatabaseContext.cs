namespace LogEvac.Contexts;

using LogEvac.Models;
using Microsoft.EntityFrameworkCore;

public class SerilogDatabaseContext : BaseDatabaseContext<SerilogDatabaseContext>
{
    public SerilogDatabaseContext(DbContextOptions<SerilogDatabaseContext> options) : base(options)
    {
    }

    public DbSet<LogEvent> LogEvents { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<LogEvent>(entity =>
        {
            entity.ToTable("LogEvents");
            entity.HasKey(e => e.Id);
        });

        base.OnModelCreating(modelBuilder);
    }
}
