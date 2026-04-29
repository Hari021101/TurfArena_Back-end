using Microsoft.EntityFrameworkCore;
using TurfArena.Data;
using TurfArena.Models;

namespace TurfArena.Services;

public class SlotGeneratorService
{
    private readonly AppDbContext _db;

    public SlotGeneratorService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<List<Slot>> GetOrGenerateSlotsAsync(string turfId, string date)
    {
        // Check if slots already exist for this date
        var existingSlots = await _db.Slots
            .Where(s => s.TurfId == turfId && s.Date == date)
            .ToListAsync();

        if (existingSlots.Any())
        {
            return existingSlots;
        }

        // Get turf to know timing
        var turf = await _db.Turfs.FindAsync(turfId);
        if (turf == null) return new List<Slot>();

        // Generate slots
        var slots = new List<Slot>();
        
        if (TimeSpan.TryParse(turf.TimingStart, out var startTime) && 
            TimeSpan.TryParse(turf.TimingEnd, out var endTime))
        {
            var currentTime = startTime;
            // Handle cross-midnight by adding 24 hours if end time is less than start time
            var actualEndTime = endTime <= startTime ? endTime.Add(TimeSpan.FromHours(24)) : endTime;
            
            while (currentTime < actualEndTime)
            {
                var nextTime = currentTime.Add(TimeSpan.FromHours(1));
                
                var formatTime = (TimeSpan t) => $"{(t.Hours % 24):D2}:{t.Minutes:D2}";
                
                var timeString = $"{formatTime(currentTime)} - {formatTime(nextTime)}";
                
                slots.Add(new Slot
                {
                    TurfId = turfId,
                    Date = date,
                    Time = timeString,
                    Status = "available",
                    Price = turf.PricePerHour
                });

                currentTime = nextTime;
            }

            _db.Slots.AddRange(slots);
            await _db.SaveChangesAsync();
        }

        return slots;
    }
}
