namespace TurfArena.DTOs.Booking;

public class CreateBookingDto
{
    public string TurfId { get; set; } = string.Empty;
    public string Date { get; set; } = string.Empty; // YYYY-MM-DD
    public List<string> Slots { get; set; } = new();
    public decimal TotalAmount { get; set; }
}

public class BookingDto
{
    public string Id { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public string TurfId { get; set; } = string.Empty;
    public string TurfName { get; set; } = string.Empty;
    public string? TurfPhoto { get; set; }
    public string Date { get; set; } = string.Empty;
    public List<string> Slots { get; set; } = new();
    public decimal TotalAmount { get; set; }
    public string Status { get; set; } = string.Empty;
    public string PaymentStatus { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
