using System.Runtime.InteropServices.JavaScript;
using System.Text.Json;
using SolarWatch.Model;

namespace SolarWatch.JsonProcessor;

public class JsonProcessor : IJsonProcessor
{
    public Coordinate ProcessWeatherApiCityToCoordinate(string data)
    {
        JsonDocument json = JsonDocument.Parse(data);

        if (json.RootElement.ValueKind == JsonValueKind.Array && json.RootElement.GetArrayLength() > 0)
        {
            JsonElement city = json.RootElement[0];

            if (city.TryGetProperty("lon", out JsonElement lon))
            {
                if (city.TryGetProperty("lat", out JsonElement lat))
                {
                    Coordinate coordinate = new Coordinate
                    {
                        Longitude = lon.GetDouble(),
                        Latitude = lat.GetDouble()
                    };
                    return coordinate;
                }
            }
        }

        throw new JsonException("Could not get coordinates. This city does not exist in the API.");
        
    }

    public SunriseSunset ProcessSunriseSunsetApi(string city, string? date, string data)
    {
        JsonDocument json = JsonDocument.Parse(data);

        DateTime dateTime = DateParser(date);
        Console.WriteLine(dateTime);
        
        if (json.RootElement.ValueKind == JsonValueKind.Object)
        {
            JsonElement result = json.RootElement.GetProperty("results");

            if (result.TryGetProperty("sunrise", out JsonElement sunrise))
            {
                if (result.TryGetProperty("sunset", out JsonElement sunset))
                {
                    string sunriseTo24HoursFrom12 = ConvertAmPmTimeTo24Hours(sunrise.ToString());
                    string sunsetTo24HoursFrom12 = ConvertAmPmTimeTo24Hours(sunset.ToString());
                    return new SunriseSunset(city, dateTime, sunriseTo24HoursFrom12, sunsetTo24HoursFrom12);
                }
            }
        }
        throw new JsonException("Could not get sunrise/sunset information from API.");
    }

    private string ConvertAmPmTimeTo24Hours(string time)
    {
        DateTime date = DateTime.Parse(time);

        return date.ToString("HH:mm:ss");
    }

    private DateTime DateParser(string? date)
    {
        if (string.IsNullOrEmpty(date))
        {
            return DateTime.Today;
        }
        else
        {
            if(!DateTime.TryParse(date, out var dateTime))
            {
                throw new FormatException("Date format is not correct.");
            }
            return dateTime;
        }
    }
}