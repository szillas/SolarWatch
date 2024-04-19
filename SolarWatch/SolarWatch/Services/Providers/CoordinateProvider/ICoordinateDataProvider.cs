namespace SolarWatch.Services.Providers.CoordinateProvider;

public interface ICoordinateDataProvider
{
    Task<string> GetCityFromOpenWeatherMap(string city);
    Task<string> GetCityFromOpenWeatherMap(string city, string country);
    Task<string> GetCityFromOpenWeatherMap(string city, string country, string state);
}