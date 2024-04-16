namespace SolarWatch.Services.Providers.CoordinateProvider;

public interface ICoordinateDataProvider
{
    Task<string> GetCityFromOpenWeatherMap(string city);
}