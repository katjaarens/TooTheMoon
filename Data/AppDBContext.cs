using Microsoft.EntityFrameworkCore;
using TooTheMoon.Models;

namespace TooTheMoon.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
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

        modelBuilder.Entity<Groomsmaid>(entity =>
        {
            entity.ToTable("groomsmaids", "public");

            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id)
                .HasColumnName("id")
                .UseIdentityByDefaultColumn();

            entity.Property(e => e.Name)
                .HasColumnName("Name");

            entity.Property(e => e.Description)
                .HasColumnName("Description");

            entity.Property(e => e.Phone)
                .HasColumnName("Phone");

            entity.Property(e => e.ImagePath)
                .HasColumnName("ImagePath");

            entity.Property(e => e.RoleBadge)
                .HasColumnName("rolebadge");

            entity.Property(e => e.Anecdote)
                .HasColumnName("anecdote");

            entity.Property(e => e.FirstImpression)
                .HasColumnName("firstimpression");

            entity.Property(e => e.SinceWhen)
                .HasColumnName("sincewhen");

            entity.Property(e => e.Speciality)
                .HasColumnName("speciality");
        });
    }
}
