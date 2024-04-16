using System.Text.Json;
using NUnit.Framework;
using SolarWatch.Model;
using SolarWatch.Model.NotInUse;
using SolarWatch.Services.JsonProcessor;

namespace SolarWatchTest;

[TestFixture]
public class JsonProcessorTest
{
    private JsonProcessor _jsonProcessor;

    private City _testCity = new City
    {
        Name = "London",
        Latitude = 10,
        Longitude = 10,
        Country = "UK",
        State = "UK"
    };
    private DateTime _testDateTime = new DateTime(2024, 01, 01);

    [SetUp]
    public void Setup()
    {
        _jsonProcessor = new JsonProcessor();
    }

    [Test]
    public void ProcessWeatherApiCityToCoordinateReturnsCorrectCityFromString()
    {
        // Arrange
        string data = "[{\"name\": \"Budapest\",\"lon\": -0.1276, \"lat\": 51.5074, \"country\": \"HU\"}]";
        string data2 = "[{\"name\": \"Budapest\",\"lon\": -0.1276, \"lat\": 51.5074, \"country\": \"HU\", \"state\": \"EU\"}]";

        City city = _jsonProcessor.ProcessWeatherApiCityStringToCity(data);
        City city2 = _jsonProcessor.ProcessWeatherApiCityStringToCity(data2);

        // Assert
        Assert.IsNotNull(city);
        Assert.That(city.Latitude, Is.EqualTo(51.5074));
        Assert.That(city.State, Is.EqualTo(null));
        Assert.IsNotNull(city2);
        Assert.That(city2.Latitude, Is.EqualTo(51.5074));
        Assert.That(city2.State, Is.EqualTo("EU"));
    }
    
    [Test]
    public void ProcessWeatherApiCityToCoordinateEmptyDataThrowsJsonException()
    {
        // Arrange
        string data = "[]"; // Empty JSON array

        // Act and Assert
        Assert.Throws<JsonException>(() => _jsonProcessor.ProcessWeatherApiCityStringToCity(data));
    }

    [Test]
    public void ProcessWeatherApiCityToCoordinateMissingLonPropertyThrowsJsonException()
    {
        // Arrange
        string data = "[{\"lat\": 51.5074}]"; // Missing "lon" property

        // Act and Assert
        Assert.Throws<JsonException>(() => _jsonProcessor.ProcessWeatherApiCityStringToCity(data));
    }

    [Test]
    public void ProcessWeatherApiCityToCoordinateMissingLatPropertyThrowsJsonException()
    {
        // Arrange
        string data = "[{\"lon\": -0.1276}]";

        // Act and Assert
        Assert.Throws<JsonException>(() => _jsonProcessor.ProcessWeatherApiCityStringToCity(data));
    }
    
    [Test]
    public void ProcessSunriseSunsetApiReturnsSunriseSunsetCorrectly()
    {
        //    public SunriseSunset ProcessSunriseSunsetApi(string city, string? date, string data)
        // Arrange
        string data = "{\n  \"results\": {\n    \"sunrise\": \"5:59:53 AM\",\n    \"sunset\": \"6:42:17 PM\"\n  },\n  \"status\": \"OK\",\n  \"tzid\": \"UTC\"\n}";

        // Act
        SunriseSunsetOfCity sunriseSunset = _jsonProcessor.ProcessSunriseSunsetApiStringToSunriseSunset(_testCity, _testDateTime, data);

        // Assert
        Assert.IsNotNull(sunriseSunset);
        Assert.That(sunriseSunset.Sunrise, Is.EqualTo("05:59:53"));
        Assert.That(sunriseSunset.Sunset, Is.EqualTo("18:42:17"));
        Assert.That(sunriseSunset.Date, Is.EqualTo(_testDateTime));
    }
    
    /*[Test]
    public void ProcessSunriseSunsetApiThrowsNewFormatExceptionIfDateFormatIsNotCorrect()
    {
        // Arrange
        string data = "{\n  \"results\": {\n    \"sunrise\": \"5:59:53 AM\",\n    \"sunset\": \"6:42:17 PM\"\n  },\n  \"status\": \"OK\",\n  \"tzid\": \"UTC\"\n}";
        string city = "London";
        string date = "notCorrectDate";

        // Assert
        Assert.Throws<FormatException>(() =>  _jsonProcessor.ProcessSunriseSunsetApi(city, date, data));
    }*/
    
    
    [Test]
    public void ProcessSunriseSunsetApiThrowsNewFormatExceptionIfDataIsNotObject()
    {
        // Arrange
        string data = "[]";


        // Act and Assert
        Assert.Throws<JsonException>(() => _jsonProcessor.ProcessSunriseSunsetApiStringToSunriseSunset(_testCity, _testDateTime, data));
    }

    [Test]
    public void ProcessSunriseSunsetApiThrowsNewFormatExceptionIfSunriseIsNotInTheApiCallResult()
    {
        // Arrange
        string data = "{\n  \"results\": {\n   \"sunset\": \"6:42:17 PM\"\n  },\n  \"status\": \"OK\",\n  \"tzid\": \"UTC\"\n}";

        // Act and Assert
        Assert.Throws<JsonException>(() => _jsonProcessor.ProcessSunriseSunsetApiStringToSunriseSunset(_testCity, _testDateTime, data));
    }

    [Test]
    public void ProcessSunriseSunsetApiThrowsNewFormatExceptionIfSunsetIsNotInTheApiCallResult()
    {
        // Arrange
        string data = "{\n  \"results\": {\n    \"sunrise\": \"5:59:53 AM\"\n  },\n  \"status\": \"OK\",\n  \"tzid\": \"UTC\"\n}";

        // Act and Assert
        Assert.Throws<JsonException>(() => _jsonProcessor.ProcessSunriseSunsetApiStringToSunriseSunset(_testCity, _testDateTime, data));
    }

}