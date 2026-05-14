namespace TurfArena.Models;

public class Review
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string UserId { get; set; } = string.Empty;
    public AppUser User { get; set; } = null!;

    // Denormalized for display
    public string? UserName { get; set; }
    public string? UserPhoto { get; set; }

    public string TurfId { get; set; } = string.Empty;
    public Turf Turf { get; set; } = null!;
    public string BookingId { get; set; } = string.Empty;

    public int Rating { get; set; } // 1-5
    public string Comment { get; set; } = string.Empty;
    public List<string>? Photos { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
