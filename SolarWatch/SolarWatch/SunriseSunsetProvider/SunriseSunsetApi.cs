using System.Net;

namespace SolarWatch.SunriseSunsetProvider;

public class SunriseSunsetApi : ISunriseSunsetProvider
{
    private readonly ILogger<SunriseSunsetApi> _logger;

    public SunriseSunsetApi(ILogger<SunriseSunsetApi> logger)
    {
        _logger = logger;
    }

    public string GetSunriseSunset(double lat, double lon, string date)
    {

        var url = $"https://api.sunrise-sunset.org/json?lat={lat}&lng={lon}&date={date}";
        
        using var client = new WebClient();

        _logger.LogInformation("Calling Sunrise/Sunset API with url: {url}", url);
        
        return client.DownloadString(url);
    }
}