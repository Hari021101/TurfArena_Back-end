namespace TurfArena.Models;

public class Turf
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Name { get; set; } = string.Empty;
    public string OwnerId { get; set; } = string.Empty;
    public AppUser Owner { get; set; } = null!;

    // Location (owned entity — stored in same table)
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public string Address { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string? StateId { get; set; }
    public string? RegionId { get; set; }

    public string TimingStart { get; set; } = "06:00";
    public string TimingEnd { get; set; } = "23:00";

    public decimal PricePerHour { get; set; }
    public string Description { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public double Rating { get; set; } = 0;
    public int ReviewCount { get; set; } = 0;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    // JSON-serialized arrays stored as strings
    public string PhotosJson { get; set; } = "[]";
    public string AmenitiesJson { get; set; } = "[]";
    public string SportTypesJson { get; set; } = "[]";

    // Navigation
    public List<Slot> Slots { get; set; } = new();
    public List<Booking> Bookings { get; set; } = new();
    public List<Review> Reviews { get; set; } = new();
    public List<FavoriteTurf> FavoritedBy { get; set; } = new();
}
