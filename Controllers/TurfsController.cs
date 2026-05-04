namespace TurfArena.Controllers;

using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TurfArena.Data;
using TurfArena.DTOs.Turf;
using TurfArena.Models;

[ApiController]
[Route("api/turfs")]
public class TurfsController : ControllerBase
{
    private readonly AppDbContext _db;

    public TurfsController(AppDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllTurfs([FromQuery] string? city)
    {
        var query = _db.Turfs.Where(t => t.IsActive);
        
        if (!string.IsNullOrWhiteSpace(city))
        {
            query = query.Where(t => t.City.ToLower() == city.ToLower());
        }

        var turfs = await query.ToListAsync();
        var dtos = turfs.Select(MapToDto).ToList();
        return Ok(dtos);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetTurf(string id)
    {
        var turf = await _db.Turfs.FirstOrDefaultAsync(t => t.Id == id);
        if (turf == null) return NotFound();
        return Ok(MapToDto(turf));
    }

    [HttpGet("owner/{ownerId}")]
    [Authorize]
    public async Task<IActionResult> GetTurfsByOwner(string ownerId)
    {
        var turfs = await _db.Turfs.Where(t => t.OwnerId == ownerId).ToListAsync();
        return Ok(turfs.Select(MapToDto).ToList());
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> CreateTurf([FromBody] CreateTurfDto dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
        if (userId == null) return Unauthorized();

        var turf = new Turf
        {
            Name = dto.Name,
            OwnerId = userId,
            Latitude = dto.Location.Latitude,
            Longitude = dto.Location.Longitude,
            Address = dto.Location.Address,
            City = dto.Location.City,
            StateId = dto.StateId,
            RegionId = dto.RegionId,
            TimingStart = dto.Timing.Start,
            TimingEnd = dto.Timing.End,
            PricePerHour = dto.PricePerHour,
            Description = dto.Description,
            PhotosJson = JsonSerializer.Serialize(dto.Photos ?? new List<string>()),
            AmenitiesJson = JsonSerializer.Serialize(dto.Amenities ?? new List<string>()),
            SportTypesJson = JsonSerializer.Serialize(dto.SportTypes ?? new List<string>()),
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };

        _db.Turfs.Add(turf);
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetTurf), new { id = turf.Id }, MapToDto(turf));
    }

    [HttpPut("{id}")]
    [Authorize]
    public async Task<IActionResult> UpdateTurf(string id, [FromBody] UpdateTurfDto dto)
    {
        var turf = await _db.Turfs.FindAsync(id);
        if (turf == null) return NotFound();

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
        if (turf.OwnerId != userId) return Forbid();

        if (dto.Name != null) turf.Name = dto.Name;
        if (dto.Location != null)
        {
            turf.Latitude = dto.Location.Latitude;
            turf.Longitude = dto.Location.Longitude;
            turf.Address = dto.Location.Address;
            turf.City = dto.Location.City;
        }
        if (dto.Timing != null)
        {
            turf.TimingStart = dto.Timing.Start;
            turf.TimingEnd = dto.Timing.End;
        }
        if (dto.PricePerHour.HasValue) turf.PricePerHour = dto.PricePerHour.Value;
        if (dto.Description != null) turf.Description = dto.Description;
        if (dto.Photos != null) turf.PhotosJson = JsonSerializer.Serialize(dto.Photos);
        if (dto.Amenities != null) turf.AmenitiesJson = JsonSerializer.Serialize(dto.Amenities);
        if (dto.SportTypes != null) turf.SportTypesJson = JsonSerializer.Serialize(dto.SportTypes);
        if (dto.IsActive.HasValue) turf.IsActive = dto.IsActive.Value;

        turf.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();

        return Ok(MapToDto(turf));
    }

    [HttpDelete("{id}")]
    [Authorize]
    public async Task<IActionResult> DeleteTurf(string id)
    {
        var turf = await _db.Turfs.FindAsync(id);
        if (turf == null) return NotFound();

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
        if (turf.OwnerId != userId) return Forbid();

        turf.IsActive = false; // Soft delete
        turf.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();

        return NoContent();
    }

    private static TurfDto MapToDto(Turf t)
    {
        return new TurfDto
        {
            Id = t.Id,
            Name = t.Name,
            OwnerId = t.OwnerId,
            Location = new TurfLocationDto
            {
                Latitude = t.Latitude,
                Longitude = t.Longitude,
                Address = t.Address,
                City = t.City
            },
            Timing = new TurfTimingDto
            {
                Start = t.TimingStart,
                End = t.TimingEnd
            },
            PricePerHour = t.PricePerHour,
            Description = t.Description,
            IsActive = t.IsActive,
            Rating = t.Rating,
            ReviewCount = t.ReviewCount,
            StateId = t.StateId,
            RegionId = t.RegionId,
            CreatedAt = t.CreatedAt,
            Photos = DeserializeList(t.PhotosJson),
            Amenities = DeserializeList(t.AmenitiesJson),
            SportTypes = DeserializeList(t.SportTypesJson)
        };
    }

    private static List<string> DeserializeList(string json)
    {
        try
        {
            return string.IsNullOrWhiteSpace(json) ? new List<string>() : JsonSerializer.Deserialize<List<string>>(json) ?? new List<string>();
        }
        catch
        {
            return new List<string>();
        }
    }
}
