using Microsoft.EntityFrameworkCore;

namespace MiniTwit.Database;

public class MiniTwitContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Message> Messages { get; set; }
    public string DbPath { get; }

    public MiniTwitContext(IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            var path = Directory.GetCurrentDirectory();
            DbPath = System.IO.Path.Join(path, "Database/minitwit.db");
        }
        else
        {
            var folder = Environment.SpecialFolder.LocalApplicationData;
            var path = Environment.GetFolderPath(folder);
            DbPath = System.IO.Path.Join(path, "minitwit.db");
        }
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