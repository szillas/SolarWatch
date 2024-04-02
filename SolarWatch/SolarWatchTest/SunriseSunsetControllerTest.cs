using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SolarWatch;
using SolarWatch.Controllers;
using SolarWatch.CoordinateProvider;
using SolarWatch.JsonProcessor;
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
    
    
    /*[Test]
    public void GetSunriseSunsetReturnsBadRequestResultIfCityToCoordinateMethodFails()
    {
        //Arrange
        _jsonProcessorMock.Setup(x => x.ProcessWeatherApiCityToCoordinate(It.IsAny<string>()))
            .Throws(new JsonException());
        
        //Act
        var result = _controller.GetSunriseSunset("BP", null);
        
        //Assert
        Assert.IsInstanceOf(typeof(BadRequestResult), result.Result);
    }*/
}