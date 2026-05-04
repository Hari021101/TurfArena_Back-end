using Microsoft.AspNetCore.Mvc;
using TurfArena.Services;

namespace TurfArena.Controllers;

[ApiController]
[Route("api/slots")]
public class SlotsController : ControllerBase
{
    private readonly SlotGeneratorService _slotGenerator;

    public SlotsController(SlotGeneratorService slotGenerator)
    {
        _slotGenerator = slotGenerator;
    }

    [HttpGet("{turfId}")]
    public async Task<IActionResult> GetSlots(string turfId, [FromQuery] string date)
    {
        if (string.IsNullOrWhiteSpace(date))
            return BadRequest(new { message = "Date query parameter is required." });

        var slots = await _slotGenerator.GetOrGenerateSlotsAsync(turfId, date);
        return Ok(slots);
    }
}
