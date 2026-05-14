using Microsoft.AspNetCore.Mvc;
using TurfArena.Data;
using TurfArena.Services;

namespace TurfArena.Controllers;

[ApiController]
[Route("api/weather")]
public class WeatherController : ControllerBase
{
    private readonly WeatherService _weatherService;
    private readonly AppDbContext _db;

    public WeatherController(WeatherService weatherService, AppDbContext db)
    {
        _weatherService = weatherService;
        _db = db;
    }

    [HttpGet("{turfId}")]
    public async Task<IActionResult> GetWeather(string turfId)
    {
        var turf = await _db.Turfs.FindAsync(turfId);
        if (turf == null) return NotFound(new { message = "Turf not found." });

        var weather = await _weatherService.GetWeatherAsync(turf.Latitude, turf.Longitude);
        
        if (weather == null)
            return StatusCode(500, new { message = "Weather data unavailable." });

        return Ok(weather);
    }
}
