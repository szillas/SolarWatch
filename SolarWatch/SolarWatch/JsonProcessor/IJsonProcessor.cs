using SolarWatch.Model;
using SolarWatch.Model.NotInUse;

namespace SolarWatch.JsonProcessor;

public interface IJsonProcessor
{
    City ProcessWeatherApiCityStringToCity(string data);
    
    SunriseSunsetOfCity ProcessSunriseSunsetApiStringToSunriseSunset(City city, DateTime date, string data);
}