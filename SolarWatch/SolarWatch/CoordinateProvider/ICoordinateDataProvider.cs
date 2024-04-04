namespace SolarWatch.CoordinateProvider;

public interface ICoordinateDataProvider
{
    Task<string> GetCityFromOpenWeatherMap(string city);
}