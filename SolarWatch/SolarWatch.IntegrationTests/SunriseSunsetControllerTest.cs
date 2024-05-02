using System.Globalization;
using System.Net;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using SolarWatch.Data;
using SolarWatch.Model;
using SolarWatch.Services.JsonProcessor;
using SolarWatch.Services.Providers.CoordinateProvider;
using SolarWatch.Services.Providers.SunriseSunsetProvider;
using Xunit.Abstractions;

namespace SolarWatch.IntegrationTests;

[Collection("Integration Tests")]
public class SunriseSunsetControllerTest : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _httpClient;
    private readonly ITestOutputHelper _outputHelper;
    private readonly CustomWebApplicationFactory _factory;
    private readonly Mock<ICoordinateDataProvider> _coordinateDataProviderMock;
    private readonly Mock<IJsonProcessor> _jsonProcessorMock;
    private readonly Mock<ISunriseSunsetProvider> _sunriseSunsesProvider;
    
    

    public SunriseSunsetControllerTest(ITestOutputHelper outputHelper)
    {
        _coordinateDataProviderMock = new Mock<ICoordinateDataProvider>();
        _jsonProcessorMock = new Mock<IJsonProcessor>();
        _sunriseSunsesProvider = new Mock<ISunriseSunsetProvider>();
        _factory = new CustomWebApplicationFactory();
        _factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureTestServices(services =>
            {
                services.AddSingleton<ICoordinateDataProvider>(_ => _coordinateDataProviderMock.Object);
                services.AddSingleton<IJsonProcessor>(_ => _jsonProcessorMock.Object);
                services.AddSingleton<ISunriseSunsetProvider>(_ => _sunriseSunsesProvider.Object);
                
            });
        });
        _httpClient = _factory.CreateClient();
        _outputHelper = outputHelper;
    }
    
    
    [Fact]
    public void SeededDataIsPresentInDatabase()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<SolarWatchApiContext>();

        // Act - Query the database
        var cities = dbContext.Cities.ToList();
        var sunriseSunsetOfCities = dbContext.SunriseSunsetOfCities.ToList();

        // Assert - Check if the seeded items are present
        Assert.NotNull(cities);
        Assert.NotNull(sunriseSunsetOfCities);
        
        Assert.NotEmpty(cities); 
        Assert.NotEmpty(sunriseSunsetOfCities);
    }
    
    [Fact]
    public async Task GetSunriseSunsetReturnsNotFoundResultIfCityNotFound()
    {
        // Arrange
        // Act
        var response = await _httpClient.GetAsync("/api/SunriseSunset/GetSunriseSunset?cityName=fgnfghjghjfgvhbnbfghvnhj");

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
    
    [Fact]
    public async Task GetSunriseSunsetReturnsBadRequestResultIfSunsetProviderFails()
    {
        // Arrange
        var city = "Budapest";
        var invalidDateTime = "invalidDateTime";
        
        // Set up mock behavior
        _coordinateDataProviderMock.Setup(x => x.GetCityFromOpenWeatherMap(city))
            .Throws(new Exception("Failed to get city from API"));

        // Act
        var response = await _httpClient.GetAsync($"/api/sunrisesunset/GetSunriseSunset?cityName={city}&date={invalidDateTime}");
        var responseContent = await response.Content.ReadAsStringAsync();
        _outputHelper.WriteLine(responseContent);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
    
    [Fact]
    public async Task GetSunriseSunsetReturnsBadRequestResultIfTimeFormatIsInvalid()
    {
        // Arrange
        var city = "London";
        var invalidDateTime = "invalidDateTime";
        
        // Act
        var response = await _httpClient.GetAsync($"/api/sunrisesunset/GetSunriseSunset?cityName={city}&date={invalidDateTime}");

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
    
    [Fact]
    public async Task GetSunriseSunsetReturnsOkIfCityAndSunriseSunsetIsInsideDatabase()
    {
        // Arrange
        var city = "London";
        var datetime = "2024-05-01";
        
        // Act
        var response = await _httpClient.GetAsync($"/api/Sunrisesunset/GetSunriseSunset?cityName={city}&date={datetime}");

        // Assert
        response.EnsureSuccessStatusCode(); // Status Code 200-299
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var responseContent = await response.Content.ReadAsStringAsync();
        _outputHelper.WriteLine(responseContent);
        Assert.Contains("London", responseContent);
        Assert.Contains("20:00", responseContent);
    }
    
    /*[Fact]
    public async Task GetSunriseSunsetReturnsOkIfCityIsNotInDbAndSunriseSunsetIsAlsoNotInDb()
    {
        // Arrange
        var city = "C";
        var datetime = "2024-05-02";
        City berlin = new City
        {
            Country = "DE",
            Longitude = 0,
            Latitude = 0,
            Name = "C",
            State = "G"
        };
        
        SunriseSunsetOfCity expectedResult = new SunriseSunsetOfCity
        {
            City = berlin,
            Date = DateTime.Today,
            Sunrise = "sunrise",
            Sunset = "sunset",
            TimeZone = "UTC"
        };
        
        _coordinateDataProviderMock.Setup(x => x.GetCityFromOpenWeatherMap(It.IsAny<string>()))
            .ReturnsAsync(city);
        _jsonProcessorMock.Setup(x => x.ProcessWeatherApiCityStringToCity(city))
            .ReturnsAsync(berlin);
        /*_sunriseSunsesProvider.Setup(x => x.GetSunriseSunset(berlin.Latitude, berlin.Longitude, It.IsAny<string>()))
            .ReturnsAsync(datetime);
        _jsonProcessorMock.Setup(x =>
                x.ProcessSunriseSunsetApiStringToSunriseSunset(berlin, DateTime.Today, datetime))
            .Returns(expectedResult);*/
        
        // Act
        /*var response = await _httpClient.GetAsync($"/api/Sunrisesunset/GetSunriseSunset?cityName={city}&date={datetime}");

        // Assert
        response.EnsureSuccessStatusCode(); // Status Code 200-299
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var responseContent = await response.Content.ReadAsStringAsync();
        _outputHelper.WriteLine(responseContent);
        Assert.Contains("London", responseContent);
        Assert.Contains("20:00", responseContent);
    }*/
    

    
    
    
}