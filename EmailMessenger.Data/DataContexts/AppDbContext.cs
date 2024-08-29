using EmailMessenger.Models.Data;
using EmailMessenger.Models.Data.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace EmailMessenger.Data.DataContexts;

public class AppDbContext : DbContext, IDataSource
{
    private readonly DatabaseSettings _settings;
    public AppDbContext(DbContextOptions<AppDbContext> options, IOptions<DatabaseSettings> settings) : base(options)
    {
        _settings = settings.Value;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(_settings.DatabaseSchema);

        modelBuilder.Entity<Recipient>().HasAlternateKey(u => u.Address);
    }

    public DbSet<Message> Messages
    {
        get; set;
    } = null!;
    public DbSet<Recipient> Recipients
    {
        get; set;
    } = null!;
    public DbSet<MessageEvent> MessageEvents
    {
        get; set;
    } = null!;
}
