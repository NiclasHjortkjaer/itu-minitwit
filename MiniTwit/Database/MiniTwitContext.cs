using Microsoft.EntityFrameworkCore;

namespace MiniTwit.Database;

public class MiniTwitContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Message> Messages { get; set; }
    public string DbPath { get; }

    public MiniTwitContext()
    {
        var path = Directory.GetCurrentDirectory();
        DbPath = System.IO.Path.Join(path, "Database/minitwit.db");
    }
    protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseSqlite($"Data Source={DbPath}");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>()
            .HasMany(c => c.Follows)
            .WithMany(c => c.Followers);
    }
}