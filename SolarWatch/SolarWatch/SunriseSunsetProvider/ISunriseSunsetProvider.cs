using SolarWatch.Model;

namespace SolarWatch.SunriseSunsetProvider;

public interface ISunriseSunsetProvider
{
    //public Task<string> GetSunriseSunset(Coordinate cityCoord, string date);
    Task<string> GetSunriseSunset(double latitude, double longitude, string? date);
}