namespace SolarWatch.SunriseSunsetProvider;

public interface ISunriseSunsetProvider
{
    string GetSunriseSunset(double lat, double lon, string date);
}