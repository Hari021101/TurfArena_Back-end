using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TurfArena.Data;
using TurfArena.DTOs.Booking;
using TurfArena.Models;

namespace TurfArena.Controllers;

[ApiController]
[Route("api/bookings")]
[Authorize]
public class BookingsController : ControllerBase
{
    private readonly AppDbContext _db;

    public BookingsController(AppDbContext db)
    {
        _db = db;
    }

    [HttpPost]
    public async Task<IActionResult> CreateBooking([FromBody] CreateBookingDto dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
        if (userId == null) return Unauthorized();

        var turf = await _db.Turfs.FindAsync(dto.TurfId);
        if (turf == null) return NotFound(new { message = "Turf not found." });

        // Check if slots are available
        var slots = await _db.Slots
            .Where(s => s.TurfId == dto.TurfId && s.Date == dto.Date && dto.Slots.Contains(s.Time))
            .ToListAsync();

        if (slots.Count != dto.Slots.Count || slots.Any(s => s.Status != "available"))
        {
            return BadRequest(new { message = "One or more selected slots are not available." });
        }

        // Mark slots as booked
        foreach (var slot in slots)
        {
            slot.Status = "booked";
        }

        var photo = "[]";
        try {
            var photos = JsonSerializer.Deserialize<List<string>>(turf.PhotosJson);
            photo = photos?.FirstOrDefault() ?? "";
        } catch {}

        var booking = new Booking
        {
            UserId = userId,
            TurfId = dto.TurfId,
            TurfName = turf.Name,
            TurfPhoto = photo,
            Date = dto.Date,
            SlotsJson = JsonSerializer.Serialize(dto.Slots),
            TotalAmount = dto.TotalAmount,
            Status = "confirmed",
            PaymentStatus = "completed", 
            CreatedAt = DateTime.UtcNow
        };

        _db.Bookings.Add(booking);
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetBooking), new { id = booking.Id }, MapToDto(booking));
    }

    [HttpGet("my")]
    public async Task<IActionResult> GetMyBookings()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
        var bookings = await _db.Bookings
            .Where(b => b.UserId == userId)
            .OrderByDescending(b => b.CreatedAt)
            .ToListAsync();

        return Ok(bookings.Select(MapToDto).ToList());
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetBooking(string id)
    {
        var booking = await _db.Bookings.FindAsync(id);
        if (booking == null) return NotFound();

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
        var userRole = User.FindFirstValue("role");
        
        // Only owner of booking or turf owner can see
        if (booking.UserId != userId && userRole != "owner") return Forbid();

        return Ok(MapToDto(booking));
    }

    [HttpPut("{id}/cancel")]
    public async Task<IActionResult> CancelBooking(string id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
        
        var booking = await _db.Bookings.FindAsync(id);
        if (booking == null) return NotFound();
        if (booking.UserId != userId) return Forbid();
        if (booking.Status == "cancelled") return BadRequest(new { message = "Booking is already cancelled." });

        booking.Status = "cancelled";
        booking.CancelledAt = DateTime.UtcNow;

        // Free up slots
        var slotTimes = JsonSerializer.Deserialize<List<string>>(booking.SlotsJson) ?? new List<string>();
        var slots = await _db.Slots
            .Where(s => s.TurfId == booking.TurfId && s.Date == booking.Date && slotTimes.Contains(s.Time))
            .ToListAsync();

        foreach(var slot in slots)
        {
            slot.Status = "available";
        }

        await _db.SaveChangesAsync();

        return Ok(MapToDto(booking));
    }

    [HttpGet("turf/{turfId}")]
    public async Task<IActionResult> GetTurfBookings(string turfId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
        var turf = await _db.Turfs.FindAsync(turfId);
        
        if (turf == null) return NotFound();
        if (turf.OwnerId != userId) return Forbid();

        var bookings = await _db.Bookings
            .Where(b => b.TurfId == turfId)
            .OrderByDescending(b => b.CreatedAt)
            .ToListAsync();

        return Ok(bookings.Select(MapToDto).ToList());
    }

    private static BookingDto MapToDto(Booking b)
    {
        List<string> slots = new();
        try { slots = JsonSerializer.Deserialize<List<string>>(b.SlotsJson) ?? new List<string>(); } catch {}

        return new BookingDto
        {
            Id = b.Id,
            UserId = b.UserId,
            TurfId = b.TurfId,
            TurfName = b.TurfName,
            TurfPhoto = b.TurfPhoto,
            Date = b.Date,
            Slots = slots,
            TotalAmount = b.TotalAmount,
            Status = b.Status,
            PaymentStatus = b.PaymentStatus,
            CreatedAt = b.CreatedAt
        };
    }
}
