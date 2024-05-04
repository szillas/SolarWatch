using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SolarWatch.Controllers;
using SolarWatch.Model;
using SolarWatch.Services.JsonProcessor;
using SolarWatch.Services.Providers.CoordinateProvider;
using SolarWatch.Services.Providers.SunriseSunsetProvider;
using SolarWatch.Services.Repository;

namespace SolarWatchTest;

[TestFixture]
public class SunriseSunsetControllerUnitTest
{
    private Mock<ILogger<SunriseSunsetController>> _loggerMock;
    private Mock<ICoordinateDataProvider> _coordinateDataProviderMock;
    private Mock<ISunriseSunsetProvider> _sunriseSunsetProviderMock;
    private Mock<IJsonProcessor> _jsonProcessorMock;
    private Mock<ICityRepository> _cityRepositoryMock;
    private Mock<ISunriseSunsetRepository> _sunriseSunsetRepositoryMock;
    private SunriseSunsetController _controller;
    
    private readonly City _testCity = new City
    {
        Name = "London",
        Latitude = 10,
        Longitude = 10,
        Country = "UK",
        State = "UK"
    };
    private readonly DateTime _testDateTime = new DateTime(2024, 01, 01);
    
    [SetUp]
    public void SetUp()
    {
        _loggerMock = new Mock<ILogger<SunriseSunsetController>>();
        _coordinateDataProviderMock = new Mock<ICoordinateDataProvider>();
        _sunriseSunsetProviderMock = new Mock<ISunriseSunsetProvider>();
        _jsonProcessorMock = new Mock<IJsonProcessor>();
        _cityRepositoryMock = new Mock<ICityRepository>();
        _sunriseSunsetRepositoryMock = new Mock<ISunriseSunsetRepository>();
        _controller = new SunriseSunsetController(_loggerMock.Object, _coordinateDataProviderMock.Object,
            _jsonProcessorMock.Object, _sunriseSunsetProviderMock.Object, _cityRepositoryMock.Object, 
            _sunriseSunsetRepositoryMock.Object);
    }

    
    [Test]
    public async Task GetSunriseSunsetReturnsNotFoundResultIfCoordinateDataProviderFails()
    {
        //Arrange
        _cityRepositoryMock.Setup(x => x.GetByName("Budapest")).ReturnsAsync(null as City);
        _coordinateDataProviderMock.Setup(x => x.GetCityFromOpenWeatherMap(It.IsAny<string>()))
            .Throws(new Exception());
        
        //Act
        var result = await _controller.GetSunriseSunset("BP", null);
        
        //Assert
        Assert.That(result.Result, Is.InstanceOf(typeof(NotFoundObjectResult)));
    }
    
    
    [Test]
    public async Task GetSunriseSunsetReturnsBadRequestResultIfCityToCoordinateMethodFails()
    {
        //Arrange
        var city = "Budapest";
        _cityRepositoryMock.Setup(x => x.GetByName("Budapest")).ReturnsAsync(null as City);
        _coordinateDataProviderMock.Setup(x => x.GetCityFromOpenWeatherMap(It.IsAny<string>()))
            .ReturnsAsync(city);
        _jsonProcessorMock.Setup(x => x.ProcessWeatherApiCityStringToCity(city))
            .Throws(new JsonException());
        
        //Act
        var result = await _controller.GetSunriseSunset(city, null);
        
        //Assert
        Assert.That(result.Result, Is.InstanceOf(typeof(BadRequestObjectResult)));
    }
    
    [Test]
    public async Task GetSunriseSunsetReturnsBadRequestResultIfSunsetProviderFails()
    {
        //Arrange
        var city = "London";
        _cityRepositoryMock.Setup(x => x.GetByName("Budapest")).ReturnsAsync(null as City);
        _coordinateDataProviderMock.Setup(x => x.GetCityFromOpenWeatherMap(It.IsAny<string>()))
            .ReturnsAsync(city);
        _jsonProcessorMock.Setup(x => x.ProcessWeatherApiCityStringToCity(city))
            .ReturnsAsync(_testCity);
        _sunriseSunsetRepositoryMock.Setup(x => x.GetByDateAndCity(city, _testDateTime))
            .ReturnsAsync(null as SunriseSunsetOfCity);
        _sunriseSunsetProviderMock.Setup(x => x.GetSunriseSunset(It.IsAny<double>(), It.IsAny<double>(), It.IsAny<string>()))
            .Throws(new FormatException());
        
        //Act
        var result = await _controller.GetSunriseSunset(city, null);
        
        //Assert
        Assert.That(result.Result, Is.InstanceOf(typeof(BadRequestObjectResult)));
    }
    
    [Test]
    public async Task GetSunriseSunsetReturnsNotFoundResultIfProcessSunriseSunsetApiMethodFails()
    {
        //Arrange
        string city = "Budapest";
        string sunriseSunset = "sun";
        _cityRepositoryMock.Setup(x => x.GetByName("Budapest")).ReturnsAsync(null as City);
        _coordinateDataProviderMock.Setup(x => x.GetCityFromOpenWeatherMap(It.IsAny<string>()))
            .ReturnsAsync(city);
        _jsonProcessorMock.Setup(x => x.ProcessWeatherApiCityStringToCity(city))
            .ReturnsAsync(_testCity);
        _sunriseSunsetRepositoryMock.Setup(x => x.GetByDateAndCity(city, _testDateTime))
            .ReturnsAsync(null as SunriseSunsetOfCity);
        _sunriseSunsetProviderMock.Setup(x => x.GetSunriseSunset(_testCity.Latitude, _testCity.Longitude, It.IsAny<string>()))
            .ReturnsAsync(sunriseSunset);
        _jsonProcessorMock.Setup(x =>
                x.ProcessSunriseSunsetApiStringToSunriseSunset(It.IsAny<City>(), It.IsAny<DateTime>(), sunriseSunset))
            .Throws(new JsonException());
        
        //Act
        var result = await _controller.GetSunriseSunset(city, null);
        
        //Assert
        Assert.That(result.Result, Is.InstanceOf(typeof(BadRequestObjectResult)));
    }
    
    
    [Test]
    public async Task GetSunriseSunsetReturnsOkResultIfDateAndCityDataIsValid()
    {
        //Arrange
        string city = "London";
        string sunriseSunset = "sun";
        SunriseSunsetOfCity expectedResult = new SunriseSunsetOfCity
        {
            City = _testCity,
            Date = _testDateTime,
            Sunrise = "sunrise",
            Sunset = "sunset",
            TimeZone = "UTC"
        };
        _cityRepositoryMock.Setup(x => x.GetByName(_testCity.Name)).ReturnsAsync(null as City);
        _coordinateDataProviderMock.Setup(x => x.GetCityFromOpenWeatherMap(_testCity.Name))
            .ReturnsAsync(city);
        _jsonProcessorMock.Setup(x => x.ProcessWeatherApiCityStringToCity(city))
            .ReturnsAsync(_testCity);
        _sunriseSunsetRepositoryMock.Setup(x => x.GetByDateAndCity(city, _testDateTime))
            .ReturnsAsync(null as SunriseSunsetOfCity);
        _sunriseSunsetProviderMock.Setup(x => x.GetSunriseSunset(_testCity.Latitude, _testCity.Longitude, It.IsAny<string>()))
            .ReturnsAsync(sunriseSunset);
        _jsonProcessorMock.Setup(x =>
                x.ProcessSunriseSunsetApiStringToSunriseSunset(_testCity, DateTime.Today, sunriseSunset))
            .Returns(expectedResult);
        
        //Act
        var result = await _controller.GetSunriseSunset(city, null);
        
        //Assert
        Assert.That(result.Result, Is.InstanceOf(typeof(OkObjectResult)));
        Assert.That(((OkObjectResult)result.Result).Value, Is.EqualTo(expectedResult));
    }
    
    
    [Test]
    public async Task GetSunriseSunsetReturnsOkResultIfDateAndCityDataIsValidAndCityIsFromDatabase()
    {
        //Arrange
        string sunriseSunset = "sun";
        SunriseSunsetOfCity expectedResult = new SunriseSunsetOfCity
        {
            City = _testCity,
            Date = _testDateTime,
            Sunrise = "sunrise",
            Sunset = "sunset",
            TimeZone = "UTC"
        };
        _cityRepositoryMock.Setup(x => x.GetByName(_testCity.Name)).ReturnsAsync(_testCity);
        _sunriseSunsetRepositoryMock.Setup(x => x.GetByDateAndCity(_testCity.Name, _testDateTime))
            .ReturnsAsync(null as SunriseSunsetOfCity);
        _sunriseSunsetProviderMock.Setup(x => x.GetSunriseSunset(_testCity.Latitude, _testCity.Longitude, It.IsAny<string>()))
            .ReturnsAsync(sunriseSunset);
        _jsonProcessorMock.Setup(x =>
                x.ProcessSunriseSunsetApiStringToSunriseSunset(_testCity, DateTime.Today, It.IsAny<string>()))
            .Returns(expectedResult);
        
        //Act
        var result = await _controller.GetSunriseSunset(_testCity.Name, null);
        var okObjectResult = result.Result as OkObjectResult;
        
        //Assert
        Assert.That(okObjectResult, Is.Not.Null);
        Assert.That(okObjectResult?.Value, Is.EqualTo(expectedResult));
    }
    
    
    [Test]
    public async Task GetSunriseSunsetReturnsOkResultIfDateAndCityDataIsValidAndFromDatabase()
    {
        //Arrange
        SunriseSunsetOfCity sunriseSunsetOfCity = new SunriseSunsetOfCity
        {
            City = _testCity,
            Date = _testDateTime,
            Sunrise = "sunrise",
            Sunset = "sunset",
            TimeZone = "UTC"
        };
        _cityRepositoryMock.Setup(x => x.GetByName(_testCity.Name)).ReturnsAsync(_testCity);
        _sunriseSunsetRepositoryMock.Setup(x => x.GetByDateAndCity(_testCity.Name, DateTime.Today))
            .ReturnsAsync(sunriseSunsetOfCity);
        
        //Act
        var result = await _controller.GetSunriseSunset(_testCity.Name, null);
        var okObjectResult = result.Result as OkObjectResult;
        
        //Assert
        Assert.That(okObjectResult, Is.Not.Null);
        Assert.That(okObjectResult?.Value, Is.EqualTo(sunriseSunsetOfCity));
    }
    
}