using Microsoft.EntityFrameworkCore;
using TurfArena.Models;

namespace TurfArena.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<AppUser> Users => Set<AppUser>();
    public DbSet<Turf> Turfs => Set<Turf>();
    public DbSet<Slot> Slots => Set<Slot>();
    public DbSet<Booking> Bookings => Set<Booking>();
    public DbSet<Review> Reviews => Set<Review>();
    public DbSet<Payment> Payments => Set<Payment>();
    public DbSet<FavoriteTurf> FavoriteTurfs => Set<FavoriteTurf>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // ── AppUser ──────────────────────────────────────────────────────────
        modelBuilder.Entity<AppUser>(e =>
        {
            e.HasKey(u => u.Id);
            e.HasIndex(u => u.Email).IsUnique();
            e.Property(u => u.Role).HasDefaultValue("player");
        });

        // ── Turf ─────────────────────────────────────────────────────────────
        modelBuilder.Entity<Turf>(e =>
        {
            e.HasKey(t => t.Id);
            e.HasOne(t => t.Owner)
             .WithMany(u => u.OwnedTurfs)
             .HasForeignKey(t => t.OwnerId)
             .OnDelete(DeleteBehavior.Cascade);

            e.Property(t => t.PricePerHour).HasColumnType("decimal(10,2)");
            e.HasIndex(t => t.City);
            e.HasIndex(t => t.IsActive);
        });

        // ── Slot ─────────────────────────────────────────────────────────────
        modelBuilder.Entity<Slot>(e =>
        {
            e.HasKey(s => s.Id);
            e.HasOne(s => s.Turf)
             .WithMany(t => t.Slots)
             .HasForeignKey(s => s.TurfId)
             .OnDelete(DeleteBehavior.Cascade);

            e.HasIndex(s => new { s.TurfId, s.Date });
            e.Property(s => s.Price).HasColumnType("decimal(10,2)");
        });

        // ── Booking ───────────────────────────────────────────────────────────
        modelBuilder.Entity<Booking>(e =>
        {
            e.HasKey(b => b.Id);
            e.HasOne(b => b.User)
             .WithMany(u => u.Bookings)
             .HasForeignKey(b => b.UserId)
             .OnDelete(DeleteBehavior.Restrict);

            e.HasOne(b => b.Turf)
             .WithMany(t => t.Bookings)
             .HasForeignKey(b => b.TurfId)
             .OnDelete(DeleteBehavior.Restrict);

            e.HasOne(b => b.Payment)
             .WithOne(p => p.Booking)
             .HasForeignKey<Payment>(p => p.BookingId)
             .OnDelete(DeleteBehavior.Cascade);

            e.Property(b => b.TotalAmount).HasColumnType("decimal(10,2)");
            e.HasIndex(b => b.UserId);
            e.HasIndex(b => b.TurfId);
            e.HasIndex(b => b.Date);
        });

        // ── Review ────────────────────────────────────────────────────────────
        modelBuilder.Entity<Review>(e =>
        {
            e.HasKey(r => r.Id);
            e.HasOne(r => r.User)
             .WithMany(u => u.Reviews)
             .HasForeignKey(r => r.UserId)
             .OnDelete(DeleteBehavior.Restrict);

            e.HasOne(r => r.Turf)
             .WithMany(t => t.Reviews)
             .HasForeignKey(r => r.TurfId)
             .OnDelete(DeleteBehavior.Cascade);

            e.HasIndex(r => r.TurfId);
        });

        // ── FavoriteTurf ──────────────────────────────────────────────────────
        modelBuilder.Entity<FavoriteTurf>(e =>
        {
            e.HasKey(f => f.Id);
            e.HasOne(f => f.User)
             .WithMany(u => u.FavoriteTurfs)
             .HasForeignKey(f => f.UserId)
             .OnDelete(DeleteBehavior.Cascade);

            e.HasOne(f => f.Turf)
             .WithMany(t => t.FavoritedBy)
             .HasForeignKey(f => f.TurfId)
             .OnDelete(DeleteBehavior.Cascade);

            e.HasIndex(f => new { f.UserId, f.TurfId }).IsUnique();
        });
    }
}
