using System.Net;
using SolarWatch.Model;

namespace SolarWatch.SunriseSunsetProvider;

public class SunriseSunsetApi : ISunriseSunsetProvider
{
    private readonly ILogger<SunriseSunsetApi> _logger;

    public SunriseSunsetApi(ILogger<SunriseSunsetApi> logger)
    {
        _logger = logger;
    }
    
    public string GetSunriseSunset(Coordinate cityCoord, string? date)
    {
        var url = $"https://api.sunrise-sunset.org/json?lat={cityCoord.Latitude}&lng={cityCoord.Longitude}";
        if (!string.IsNullOrEmpty(date))
        {
            if (DateTime.TryParse(date, out var dateValue))
            {
                _logger.LogInformation(date);
                string formattedDate = dateValue.ToString("yyyy-MM-dd");
                url += $"&date={formattedDate}";
            }
            else
            {
                throw new FormatException("Date format is not correct.");
            }
        }
        
        using var client = new WebClient();

        _logger.LogInformation("Calling Sunrise/Sunset API with url: {url}", url);
        
        return client.DownloadString(url);
    }
}