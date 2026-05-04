namespace TurfArena.Models;

public class Slot
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string TurfId { get; set; } = string.Empty;
    public Turf Turf { get; set; } = null!;
    public string Date { get; set; } = string.Empty; // YYYY-MM-DD
    public string Time { get; set; } = string.Empty; // "HH:mm - HH:mm"
    public string Status { get; set; } = "available"; // "available" | "booked"
    public decimal Price { get; set; }
}
