namespace TurfArena.Models;

public class Payment
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string BookingId { get; set; } = string.Empty;
    public Booking Booking { get; set; } = null!;
    public string UserId { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "INR";
    public string? PaymentMethod { get; set; }
    public string? TransactionId { get; set; }
    public string Status { get; set; } = "pending"; // pending | completed | failed | refunded
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? CompletedAt { get; set; }
}
