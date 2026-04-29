namespace TurfArena.Models;

public class Booking
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string UserId { get; set; } = string.Empty;
    public AppUser User { get; set; } = null!;
    public string TurfId { get; set; } = string.Empty;
    public Turf Turf { get; set; } = null!;

    // Denormalized for display
    public string TurfName { get; set; } = string.Empty;
    public string? TurfPhoto { get; set; }

    public string Date { get; set; } = string.Empty; // YYYY-MM-DD
    public string SlotsJson { get; set; } = "[]";    // JSON array of slot time strings
    public decimal TotalAmount { get; set; }

    public string Status { get; set; } = "confirmed"; // confirmed | completed | cancelled | pending_payment
    public string PaymentStatus { get; set; } = "pending"; // pending | completed | failed | refunded

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public DateTime? CancelledAt { get; set; }

    // Payment details
    public Payment? Payment { get; set; }
}
