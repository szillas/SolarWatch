namespace SolarWatch.Services.Providers.CoordinateProvider;

public class OpenWeatherCoordDataProviderApi : ICoordinateDataProvider
{
    private readonly ILogger<OpenWeatherCoordDataProviderApi> _logger;
    private readonly string _openWeatherApiKey;
    
    public OpenWeatherCoordDataProviderApi(ILogger<OpenWeatherCoordDataProviderApi> logger, IConfiguration configuration)
    {
        _logger = logger;
        _openWeatherApiKey = configuration["SolarWatch:OpenWeatherMapKey"] ?? throw new InvalidOperationException();
    }

    public async Task<string> GetCityFromOpenWeatherMap(string city)
    {
        var url = $"https://api.openweathermap.org/geo/1.0/direct?q={city}&limit=1&appid={_openWeatherApiKey}";

        using var client = new HttpClient();

        _logger.LogInformation("Calling OpenWeather API for Coordinates with url: {url}", url);

        var response = await client.GetAsync(url);
        return await response.Content.ReadAsStringAsync();
    }
    
    public async Task<string> GetCityFromOpenWeatherMap(string city, string country)
    {
        var url = $"https://api.openweathermap.org/geo/1.0/direct?q={city},{country}&limit=1&appid={_openWeatherApiKey}";

        using var client = new HttpClient();

        _logger.LogInformation("Calling OpenWeather API for Coordinates with url: {url}", url);

        var response = await client.GetAsync(url);
        return await response.Content.ReadAsStringAsync();
    }
    
    public async Task<string> GetCityFromOpenWeatherMap(string city, string country, string state)
    {
        var url = 
            $"https://api.openweathermap.org/geo/1.0/direct?q={city},{state},{country}&limit=1&appid={_openWeatherApiKey}";

        using var client = new HttpClient();

        _logger.LogInformation("Calling OpenWeather API for Coordinates with url: {url}", url);

        var response = await client.GetAsync(url);
        return await response.Content.ReadAsStringAsync();
    }
}