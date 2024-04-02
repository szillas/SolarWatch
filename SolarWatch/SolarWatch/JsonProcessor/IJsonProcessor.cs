using SolarWatch.Model;

namespace SolarWatch.JsonProcessor;

public interface IJsonProcessor
{
    Coordinate ProcessWeatherApiCityToCoordinate(string data);

    SunriseSunset ProcessSunriseSunsetApi(string city, DateTime date, string data);
}