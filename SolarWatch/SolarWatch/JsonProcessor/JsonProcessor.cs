using System.Text.Json;
using SolarWatch.Model;

namespace SolarWatch.JsonProcessor;

public class JsonProcessor : IJsonProcessor
{
    public Coordinate ProcessWeatherApiCityToCoordinate(string data)
    {
        JsonDocument json = JsonDocument.Parse(data);
        JsonElement lon = json.RootElement[0].GetProperty("lon");
        JsonElement lat = json.RootElement[0].GetProperty("lat");

        Coordinate coordinate = new Coordinate
        {
            Longitude = lon.GetDouble(),
            Latitude = lat.GetDouble()
        };

        return coordinate;
    }

    public SunriseSunset ProcessSunriseSunsetApi(string city, DateTime date, string data)
    {
        JsonDocument json = JsonDocument.Parse(data);
        JsonElement result = json.RootElement.GetProperty("results");
        string sunrise = result.GetProperty("sunrise").GetString();
        string sunset = result.GetProperty("sunset").GetString();

        SunriseSunset sunriseSunset = new SunriseSunset(city, date,
            ConvertAmPmTimeTo24Hours(sunrise),
            ConvertAmPmTimeTo24Hours(sunset));
        return sunriseSunset;
    }

    private string ConvertAmPmTimeTo24Hours(string time)
    {
        DateTime date = DateTime.Parse(time);

        return date.ToString("HH:mm:ss");
    }
}