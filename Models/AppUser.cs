namespace TurfArena.Models;

public class AppUser
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string Role { get; set; } = "player"; // "player" | "owner"
    public string? ProfilePicture { get; set; }
    public string? PushToken { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    // Navigation
    public List<Booking> Bookings { get; set; } = new();
    public List<Review> Reviews { get; set; } = new();
    public List<Turf> OwnedTurfs { get; set; } = new();
    public List<FavoriteTurf> FavoriteTurfs { get; set; } = new();
}

public class FavoriteTurf
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string TurfId { get; set; } = string.Empty;
    public AppUser User { get; set; } = null!;
    public Turf Turf { get; set; } = null!;
}
