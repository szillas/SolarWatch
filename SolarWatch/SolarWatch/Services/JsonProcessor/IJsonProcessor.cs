using SolarWatch.Model;

namespace SolarWatch.Services.JsonProcessor;

public interface IJsonProcessor
{
    Task<City> ProcessWeatherApiCityStringToCity(string data);
    
    SunriseSunsetOfCity ProcessSunriseSunsetApiStringToSunriseSunset(City city, DateTime date, string data);
}