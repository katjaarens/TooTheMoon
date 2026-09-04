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

        // Expliziter Tabellenname und automatisches Hochzählen für PostgreSQL (SERIAL / Identity)
        modelBuilder.Entity<RsvpGuest>()
            .ToTable("RsvpGuests")
            .Property(r => r.Id)
            .UseIdentityByDefaultColumn();
    }
}