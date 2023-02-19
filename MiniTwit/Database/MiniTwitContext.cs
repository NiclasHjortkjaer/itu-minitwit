using Microsoft.EntityFrameworkCore;

namespace MiniTwit.Database;

public class MiniTwitContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Message> Messages { get; set; }
    public string ConnectionString { get; }

    public MiniTwitContext()
    {
        var host = Environment.GetEnvironmentVariable("DB_HOST");
        var port = Environment.GetEnvironmentVariable("DB_PORT");
        var password = Environment.GetEnvironmentVariable("DB_PASSWORD");
        ConnectionString = $"Host={host};Port={port};Database=minitwitdb;Username=postgres;Password={password}";
    }
    protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseNpgsql(ConnectionString);

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>()
            .HasMany(c => c.Follows)
            .WithMany(c => c.Followers);
    }
}