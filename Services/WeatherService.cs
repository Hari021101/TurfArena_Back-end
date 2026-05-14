using System.Text.Json;

namespace TurfArena.Services;

public class WeatherDto
{
    public double Temp { get; set; }
    public string Condition { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;
}

public class WeatherService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _config;
    private readonly ILogger<WeatherService> _logger;

    public WeatherService(IHttpClientFactory httpClientFactory, IConfiguration config, ILogger<WeatherService> logger)
    {
        _httpClientFactory = httpClientFactory;
        _config = config;
        _logger = logger;
    }

    public async Task<WeatherDto?> GetWeatherAsync(double latitude, double longitude)
    {
        var apiKey = _config["WeatherApi:Key"];
        var baseUrl = _config["WeatherApi:BaseUrl"];

        if (string.IsNullOrWhiteSpace(apiKey))
        {
            _logger.LogWarning("Weather API Key is not configured.");
            return null;
        }

        try
        {
            var client = _httpClientFactory.CreateClient();
            var url = $"{baseUrl}?lat={latitude}&lon={longitude}&appid={apiKey}&units=metric";
            
            var response = await client.GetAsync(url);
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning($"Weather API returned {response.StatusCode}");
                return null;
            }

            var content = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(content);
            var root = doc.RootElement;

            var main = root.GetProperty("main");
            var temp = main.GetProperty("temp").GetDouble();

            var weatherArray = root.GetProperty("weather");
            if (weatherArray.GetArrayLength() > 0)
            {
                var weather = weatherArray[0];
                return new WeatherDto
                {
                    Temp = temp,
                    Condition = weather.GetProperty("main").GetString() ?? "",
                    Description = weather.GetProperty("description").GetString() ?? "",
                    Icon = weather.GetProperty("icon").GetString() ?? ""
                };
            }

            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching weather data.");
            return null;
        }
    }
}
