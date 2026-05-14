using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TurfArena.Data;
using TurfArena.DTOs.Review;
using TurfArena.Models;

namespace TurfArena.Controllers;

[ApiController]
[Route("api/reviews")]
public class ReviewsController : ControllerBase
{
    private readonly AppDbContext _db;

    public ReviewsController(AppDbContext db)
    {
        _db = db;
    }

    [HttpGet("turf/{turfId}")]
    public async Task<IActionResult> GetTurfReviews(string turfId)
    {
        var reviews = await _db.Reviews
            .Where(r => r.TurfId == turfId)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();

        return Ok(reviews.Select(MapToDto));
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> CreateReview([FromBody] CreateReviewDto dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
        if (userId == null) return Unauthorized();

        var booking = await _db.Bookings.FindAsync(dto.BookingId);
        if (booking == null || booking.UserId != userId) return BadRequest(new { message = "Invalid booking." });

        var user = await _db.Users.FindAsync(userId);

        var review = new Review
        {
            UserId = userId,
            UserName = user?.Name,
            UserPhoto = user?.ProfilePicture,
            TurfId = dto.TurfId,
            BookingId = dto.BookingId,
            Rating = dto.Rating,
            Comment = dto.Comment,
            Photos = dto.Photos,
            CreatedAt = DateTime.UtcNow
        };

        _db.Reviews.Add(review);

        // Update turf rating
        var turf = await _db.Turfs.FindAsync(dto.TurfId);
        if (turf != null)
        {
            var newCount = turf.ReviewCount + 1;
            var newRating = ((turf.Rating * turf.ReviewCount) + dto.Rating) / newCount;
            turf.Rating = Math.Round(newRating, 1);
            turf.ReviewCount = newCount;
        }

        await _db.SaveChangesAsync();

        return Ok(MapToDto(review));
    }

    private static ReviewDto MapToDto(Review r)
    {
        return new ReviewDto
        {
            Id = r.Id,
            UserId = r.UserId,
            UserName = r.UserName,
            UserPhoto = r.UserPhoto,
            TurfId = r.TurfId,
            BookingId = r.BookingId,
            Rating = r.Rating,
            Comment = r.Comment,
            Photos = r.Photos,
            CreatedAt = r.CreatedAt
        };
    }
}
