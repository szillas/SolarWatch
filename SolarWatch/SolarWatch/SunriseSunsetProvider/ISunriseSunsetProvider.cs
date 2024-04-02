using SolarWatch.Model;

namespace SolarWatch.SunriseSunsetProvider;

public interface ISunriseSunsetProvider
{
    public string GetSunriseSunset(Coordinate cityCoord, string date);
}