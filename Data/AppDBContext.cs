using Microsoft.EntityFrameworkCore;
using TooTheMoon.Models;

namespace TooTheMoon.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<RsvpGuest> RsvpGuests => Set<RsvpGuest>();
    public DbSet<WeddingTable> WeddingTables => Set<WeddingTable>();
    public DbSet<Groomsmaid> Groomsmaids => Set<Groomsmaid>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<RsvpGuest>()
            .ToTable("RsvpGuests")
            .Property(r => r.Id)
            .UseIdentityByDefaultColumn();

        modelBuilder.Entity<RsvpGuest>()
            .HasOne(g => g.WeddingTable)
            .WithMany(t => t.Guests)
            .HasForeignKey(g => g.WeddingTableId)
            .OnDelete(DeleteBehavior.SetNull);

        // Konfiguration für Trauzeugen / Groomsmaids
        modelBuilder.Entity<Groomsmaid>()
            .ToTable("Groomsmaids")
            .Property(g => g.Id)
            .UseIdentityByDefaultColumn();

            // Konfiguration für Groomsmaids (Trauzeugen)
        modelBuilder.Entity<Groomsmaid>()
            .ToTable("Groomsmaids")
            .Property(g => g.Id)
            .HasColumnName("Id") // Falls die Spalte in Postgres kleingeschrieben ist, hier anpassen (z.B. "id")
            .UseIdentityByDefaultColumn();
    }
}