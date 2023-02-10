using Duende.IdentityServer.EntityFramework.Options;
using itu_minitwit.Server.Database.Configurations;
using Microsoft.AspNetCore.ApiAuthorization.IdentityServer;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace itu_minitwit.Server.Database;

public class MinitwitContext : ApiAuthorizationDbContext<User>
{
    public DbSet<User> Users { get; set; }
    public DbSet<Message> Messages { get; set; }
    public string DbPath { get; }

    public MinitwitContext(
        DbContextOptions options,
        IOptions<OperationalStoreOptions> operationalStoreOptions) : base(options, operationalStoreOptions)
    {
        var path = Directory.GetCurrentDirectory();
        DbPath = System.IO.Path.Join(path, "Database/minitwit.db");
    }
    protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseSqlite($"Data Source={DbPath}");
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfiguration(new RoleConfiguration());

        modelBuilder.Entity<User>()
            .HasMany(c => c.Follows)
            .WithMany(c => c.Followers);
    }
}