namespace TurfArena.DTOs.Review;

public class CreateReviewDto
{
    public string TurfId { get; set; } = string.Empty;
    public string BookingId { get; set; } = string.Empty;
    public int Rating { get; set; }
    public string Comment { get; set; } = string.Empty;
}

public class ReviewDto
{
    public string Id { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public string? UserName { get; set; }
    public string? UserPhoto { get; set; }
    public string TurfId { get; set; } = string.Empty;
    public string BookingId { get; set; } = string.Empty;
    public int Rating { get; set; }
    public string Comment { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
