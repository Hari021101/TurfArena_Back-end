namespace TurfArena.DTOs.Turf;

public class TurfLocationDto
{
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public string Address { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
}

public class TurfTimingDto
{
    public string Start { get; set; } = "06:00";
    public string End { get; set; } = "23:00";
}

public class TurfDto
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string OwnerId { get; set; } = string.Empty;
    public TurfLocationDto Location { get; set; } = null!;
    public TurfTimingDto Timing { get; set; } = null!;
    public decimal PricePerHour { get; set; }
    public string Description { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public double Rating { get; set; }
    public int ReviewCount { get; set; }
    public List<string> Photos { get; set; } = new();
    public List<string> Amenities { get; set; } = new();
    public List<string> SportTypes { get; set; } = new();
    public string? StateId { get; set; }
    public string? RegionId { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateTurfDto
{
    public string Name { get; set; } = string.Empty;
    public TurfLocationDto Location { get; set; } = null!;
    public TurfTimingDto Timing { get; set; } = null!;
    public decimal PricePerHour { get; set; }
    public string Description { get; set; } = string.Empty;
    public List<string> Photos { get; set; } = new();
    public List<string> Amenities { get; set; } = new();
    public List<string> SportTypes { get; set; } = new();
    public string? StateId { get; set; }
    public string? RegionId { get; set; }
}

public class UpdateTurfDto
{
    public string? Name { get; set; }
    public TurfLocationDto? Location { get; set; }
    public TurfTimingDto? Timing { get; set; }
    public decimal? PricePerHour { get; set; }
    public string? Description { get; set; }
    public List<string>? Photos { get; set; }
    public List<string>? Amenities { get; set; }
    public List<string>? SportTypes { get; set; }
    public bool? IsActive { get; set; }
}
