using Microsoft.EntityFrameworkCore;
using TooTheMoon.Models;

namespace TooTheMoon.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<RsvpGuest> RsvpGuests => Set<RsvpGuest>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Erzwingt für PostgreSQL ein automatisches Hochzählen der ID (SERIAL / Identity)
        modelBuilder.Entity<RsvpGuest>()
            .Property(r => r.Id)
            .UseIdentityByDefaultColumn();
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    base.OnModelCreating(modelBuilder);

    modelBuilder.Entity<RsvpGuest>()
        .ToTable("RsvpGuests") // Entspricht exakt dem Tabellennamen in Postgres
        .Property(r => r.Id)
        .UseIdentityByDefaultColumn();
}
}