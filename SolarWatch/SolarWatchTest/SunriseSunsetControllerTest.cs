using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SolarWatch;
using SolarWatch.Controllers;
using SolarWatch.CoordinateProvider;
using SolarWatch.JsonProcessor;
using SolarWatch.Model;
using SolarWatch.SunriseSunsetProvider;

namespace SolarWatchTest;

[TestFixture]
public class SunriseSunsetControllerTest
{
    private Mock<ILogger<SunriseSunsetController>> _loggerMock;
    private Mock<ICoordinateDataProvider> _coordinateDataProviderMock;
    private Mock<ISunriseSunsetProvider> _sunriseSunsetProviderMock;
    private Mock<IJsonProcessor> _jsonProcessorMock;
    private SunriseSunsetController _controller;

    [SetUp]
    public void SetUp()
    {
        _loggerMock = new Mock<ILogger<SunriseSunsetController>>();
        _coordinateDataProviderMock = new Mock<ICoordinateDataProvider>();
        _sunriseSunsetProviderMock = new Mock<ISunriseSunsetProvider>();
        _jsonProcessorMock = new Mock<IJsonProcessor>();
        _controller = new SunriseSunsetController(_loggerMock.Object, _coordinateDataProviderMock.Object,
            _jsonProcessorMock.Object, _sunriseSunsetProviderMock.Object);
    }

    
    [Test]
    public void GetSunriseSunsetReturnsNotFoundResultIfCoordinateDataProviderFails()
    {
        //Arrange
        _coordinateDataProviderMock.Setup(x => x.GetCoordinate(It.IsAny<string>()))
            .Throws(new Exception());
        
        //Act
        var result = _controller.GetSunriseSunset("BP", null);
        
        //Assert
        Assert.IsInstanceOf(typeof(NotFoundObjectResult), result.Result);
    }
    
    
    [Test]
    public void GetSunriseSunsetReturnsBadRequestResultIfCityToCoordinateMethodFails()
    {
        //Arrange
        var city = "Budapest";
        _coordinateDataProviderMock.Setup(x => x.GetCoordinate(It.IsAny<string>()))
            .Returns(city);
        _jsonProcessorMock.Setup(x => x.ProcessWeatherApiCityToCoordinate(city))
            .Throws(new JsonException());
        
        //Act
        var result = _controller.GetSunriseSunset(city, null);
        
        //Assert
        Assert.IsInstanceOf(typeof(BadRequestObjectResult), result.Result);
    }
    
    [Test]
    public void GetSunriseSunsetReturnsBadRequestResultIfSunsetProviderFails()
    {
        //Arrange
        var city = "Budapest";
        Coordinate coordinate = new Coordinate();
        _coordinateDataProviderMock.Setup(x => x.GetCoordinate(It.IsAny<string>()))
            .Returns(city);
        _jsonProcessorMock.Setup(x => x.ProcessWeatherApiCityToCoordinate(city))
            .Returns(coordinate);
        _sunriseSunsetProviderMock.Setup(x => x.GetSunriseSunset(coordinate, It.IsAny<string>()))
            .Throws(new FormatException());
        
        //Act
        var result = _controller.GetSunriseSunset(city, null);
        
        //Assert
        Assert.IsInstanceOf(typeof(BadRequestObjectResult), result.Result);
    }
    
    [Test]
    public void GetSunriseSunsetReturnsNotFoundResultIfProcessSunriseSunsetApiMethodFails()
    {
        //Arrange
        string city = "Budapest";
        Coordinate coordinate = new Coordinate();
        string sunriseSunset = "sun";
        _coordinateDataProviderMock.Setup(x => x.GetCoordinate(It.IsAny<string>()))
            .Returns(city);
        _jsonProcessorMock.Setup(x => x.ProcessWeatherApiCityToCoordinate(city))
            .Returns(coordinate);
        _sunriseSunsetProviderMock.Setup(x => x.GetSunriseSunset(coordinate, It.IsAny<string>()))
            .Returns(sunriseSunset);
        _jsonProcessorMock.Setup(x =>
                x.ProcessSunriseSunsetApi(city, It.IsAny<string>(), sunriseSunset))
            .Throws(new Exception());
        
        //Act
        var result = _controller.GetSunriseSunset(city, null);
        
        //Assert
        Assert.IsInstanceOf(typeof(NotFoundObjectResult), result.Result);
    }
    
}