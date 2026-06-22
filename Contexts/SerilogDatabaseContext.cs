namespace LogEvac.Contexts;

using LogEvac.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

public class SerilogDatabaseContext : BaseDatabaseContext<SerilogDatabaseContext>
{
    private readonly LogEvacSettings _settings;
    private readonly string _tableName;

    public SerilogDatabaseContext(DbContextOptions<SerilogDatabaseContext> options, IOptions<LogEvacSettings> options1) : base(options)
    {
        _settings = options1.Value;
        _tableName = _settings.LoggingTable ?? "LogEvents";
    }

    public DbSet<LogEvent> LogEvents { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<LogEvent>(entity =>
        {
            entity.ToTable(_tableName);
            entity.HasKey(e => e.Id);
        });

        base.OnModelCreating(modelBuilder);
    }
}
