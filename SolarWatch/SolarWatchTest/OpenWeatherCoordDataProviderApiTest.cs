using System.Net;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SolarWatch.CoordinateProvider;

namespace SolarWatchTest;

[TestFixture]
public class OpenWeatherCoordDataProviderApiTest
{
    private Mock<ILogger<OpenWeatherCoordDataProviderApi>> _loggerMock;
    private ICoordinateDataProvider _coordinateDataProvider;
    private Mock<WebClient> _webClientMock;

    [SetUp]
    public void Setup()
    {
        _loggerMock = new Mock<ILogger<OpenWeatherCoordDataProviderApi>>();
        _coordinateDataProvider = new OpenWeatherCoordDataProviderApi(_loggerMock.Object);
        _webClientMock = new Mock<WebClient>();
    }

    /*[Test]
    public void GetCoordinateReturnsCorrectString()
    {
        string city = "London";
        string expectedResponse = "{\"lon\": -0.1276, \"lat\": 51.5074}";
        string apiKey = "key";
        string url = $"https://api.openweathermap.org/geo/1.0/direct?q={city}&limit=1&appid={apiKey}";

        _webClientMock.Setup(client => client.DownloadString(url)).Returns(expectedResponse);

        string result = _coordinateDataProvider.GetCoordinate(city);
        
        Assert.That(result, Is.EqualTo(expectedResponse));
    }*/
}