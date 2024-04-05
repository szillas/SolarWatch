using System.Text.Json;
using NUnit.Framework;
using SolarWatch.JsonProcessor;
using SolarWatch.Model;
using SolarWatch.Model.NotInUse;

namespace SolarWatchTest;

[TestFixture]
public class JsonProcessorTest
{
    private JsonProcessor _jsonProcessor;

    [SetUp]
    public void Setup()
    {
        _jsonProcessor = new JsonProcessor();
    }

    [Test]
    public void ProcessWeatherApiCityToCoordinateReturnsCorrectCoordinateFromString()
    {
        // Arrange
        string data = "[{\"lon\": -0.1276, \"lat\": 51.5074}]";

        // Act
        Coordinate coordinate = _jsonProcessor.ProcessWeatherApiCityToCoordinate(data);

        // Assert
        Assert.IsNotNull(coordinate);
        Assert.That(coordinate.Longitude, Is.EqualTo(-0.1276));
        Assert.That(coordinate.Latitude, Is.EqualTo(51.5074));
    }
    
    [Test]
    public void ProcessWeatherApiCityToCoordinateEmptyDataThrowsJsonException()
    {
        // Arrange
        string data = "[]"; // Empty JSON array

        // Act and Assert
        Assert.Throws<JsonException>(() => _jsonProcessor.ProcessWeatherApiCityToCoordinate(data));
    }

    [Test]
    public void ProcessWeatherApiCityToCoordinateMissingLonPropertyThrowsJsonException()
    {
        // Arrange
        string data = "[{\"lat\": 51.5074}]"; // Missing "lon" property

        // Act and Assert
        Assert.Throws<JsonException>(() => _jsonProcessor.ProcessWeatherApiCityToCoordinate(data));
    }

    [Test]
    public void ProcessWeatherApiCityToCoordinateMissingLatPropertyThrowsJsonException()
    {
        // Arrange
        string data = "[{\"lon\": -0.1276}]";

        // Act and Assert
        Assert.Throws<JsonException>(() => _jsonProcessor.ProcessWeatherApiCityToCoordinate(data));
    }
    
    [Test]
    public void ProcessSunriseSunsetApiReturnsSunriseSunsetCorrectly()
    {
        //    public SunriseSunset ProcessSunriseSunsetApi(string city, string? date, string data)
        // Arrange
        string data = "{\n  \"results\": {\n    \"sunrise\": \"5:59:53 AM\",\n    \"sunset\": \"6:42:17 PM\"\n  },\n  \"status\": \"OK\",\n  \"tzid\": \"UTC\"\n}";
        string city = "London";
        string date = "2021-01-01";

        // Act
        SunriseSunset sunriseSunset = _jsonProcessor.ProcessSunriseSunsetApi(city, date, data);

        // Assert
        Assert.IsNotNull(sunriseSunset);
        Assert.That(sunriseSunset.Sunrise, Is.EqualTo("05:59:53"));
        Assert.That(sunriseSunset.Sunset, Is.EqualTo("18:42:17"));
    }
    
    [Test]
    public void ProcessSunriseSunsetApiReturnsSunriseSunsetCorrectlyIfDateIsNull()
    {
        // Arrange
        string data = "{\n  \"results\": {\n    \"sunrise\": \"5:59:53 AM\",\n    \"sunset\": \"6:42:17 PM\"\n  },\n  \"status\": \"OK\",\n  \"tzid\": \"UTC\"\n}";
        string city = "London";
        string date = null;

        // Act
        SunriseSunset sunriseSunset = _jsonProcessor.ProcessSunriseSunsetApi(city, date, data);

        // Assert
        Assert.IsNotNull(sunriseSunset);
        Assert.That(sunriseSunset.Sunrise, Is.EqualTo("05:59:53"));
        Assert.That(sunriseSunset.Sunset, Is.EqualTo("18:42:17"));
    }
    
    [Test]
    public void ProcessSunriseSunsetApiThrowsNewFormatExceptionIfDateFormatIsNotCorrect()
    {
        // Arrange
        string data = "{\n  \"results\": {\n    \"sunrise\": \"5:59:53 AM\",\n    \"sunset\": \"6:42:17 PM\"\n  },\n  \"status\": \"OK\",\n  \"tzid\": \"UTC\"\n}";
        string city = "London";
        string date = "notCorrectDate";

        // Assert
        Assert.Throws<FormatException>(() =>  _jsonProcessor.ProcessSunriseSunsetApi(city, date, data));
    }
    
    
    [Test]
    public void ProcessSunriseSunsetApiThrowsNewFormatExceptionIfDataIsNotObject()
    {
        // Arrange
        string data = "[]";
        string city = "London";
        string date = "2021-01-01";

        // Act and Assert
        Assert.Throws<JsonException>(() => _jsonProcessor.ProcessSunriseSunsetApi(city, date, data));
    }

    [Test]
    public void ProcessSunriseSunsetApiThrowsNewFormatExceptionIfSunriseIsNotInTheApiCallResult()
    {
        // Arrange
        string data = "{\n  \"results\": {\n   \"sunset\": \"6:42:17 PM\"\n  },\n  \"status\": \"OK\",\n  \"tzid\": \"UTC\"\n}";
        string city = "London";
        string date = "2021-01-01";

        // Act and Assert
        Assert.Throws<JsonException>(() => _jsonProcessor.ProcessSunriseSunsetApi(city, date, data));
    }

    [Test]
    public void ProcessSunriseSunsetApiThrowsNewFormatExceptionIfSunsetIsNotInTheApiCallResult()
    {
        // Arrange
        string data = "{\n  \"results\": {\n    \"sunrise\": \"5:59:53 AM\"\n  },\n  \"status\": \"OK\",\n  \"tzid\": \"UTC\"\n}";
        string city = "London";
        string date = "2021-01-01";

        // Act and Assert
        Assert.Throws<JsonException>(() => _jsonProcessor.ProcessSunriseSunsetApi(city, date, data));
    }

}