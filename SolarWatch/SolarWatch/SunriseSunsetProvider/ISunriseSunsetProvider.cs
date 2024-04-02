using SolarWatch.Model;

namespace SolarWatch.SunriseSunsetProvider;

public interface ISunriseSunsetProvider
{
    public Task<string> GetSunriseSunset(Coordinate cityCoord, string date);
}