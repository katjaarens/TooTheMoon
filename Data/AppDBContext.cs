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
    }
}