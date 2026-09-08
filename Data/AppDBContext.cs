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
            .ToTable("groomsmaids")
            .Property(g => g.Id)
            .HasColumnName("id")
            .UseIdentityByDefaultColumn();

            modelBuilder.Entity<Groomsmaid>()
            .ToTable("groomsmaids")
            .Property(g => g.Id)
            .HasColumnName("id")
            .UseIdentityByDefaultColumn();

        // Mappe hier alle weiteren Spalten explizit auf ihre Kleinbuchstaben-Pendants in Postgres:
        modelBuilder.Entity<Groomsmaid>().Property(g => g.Anecdote).HasColumnName("anecdote");
        modelBuilder.Entity<Groomsmaid>().Property(g => g.FirstImpression).HasColumnName("firstimpression");
        modelBuilder.Entity<Groomsmaid>().Property(g => g.ImagePath).HasColumnName("imagepath");
        modelBuilder.Entity<Groomsmaid>().Property(g => g.Name).HasColumnName("name");
        modelBuilder.Entity<Groomsmaid>().Property(g => g.RoleBadge).HasColumnName("rolebadge");
        modelBuilder.Entity<Groomsmaid>().Property(g => g.SinceWhen).HasColumnName("sincewhen");
        modelBuilder.Entity<Groomsmaid>().Property(g => g.Speciality).HasColumnName("speciality");
    }
}