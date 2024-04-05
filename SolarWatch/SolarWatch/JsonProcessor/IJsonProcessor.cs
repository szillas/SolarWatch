using SolarWatch.Model;

namespace SolarWatch.JsonProcessor;

public interface IJsonProcessor
{
    Coordinate ProcessWeatherApiCityToCoordinate(string data);

    SunriseSunset ProcessSunriseSunsetApi(string city, string? date, string data);

    City ProcessWeatherApiCityStringToCity(string data);

    SunriseSunsetOfCity ProcessSunriseSunsetApiStringToSunriseSunset(City city, string? date, string data);

    SunriseSunsetOfCity ProcessSunriseSunsetApiStringToSunriseSunset(City city, DateTime date, string data);
}