using System.Net;

namespace SolarWatch.CoordinateProvider;

public class OpenWeatherCoordDataProviderApi : ICoordinateDataProvider
{
    private readonly ILogger<OpenWeatherCoordDataProviderApi> _logger;

    public OpenWeatherCoordDataProviderApi(ILogger<OpenWeatherCoordDataProviderApi> logger)
    {
        _logger = logger;
    }

    public string GetCoordinate(string city)
    {
        var apiKey = "2d9cacce783892837a09a4d3970b5896";
        var url = $"https://api.openweathermap.org/geo/1.0/direct?q={city}&limit=1&appid={apiKey}";
        
        using var client = new WebClient();

        _logger.LogInformation("Calling OpenWeather API for Coordinates with url: {url}", url);
        
        return client.DownloadString(url);
    }
}