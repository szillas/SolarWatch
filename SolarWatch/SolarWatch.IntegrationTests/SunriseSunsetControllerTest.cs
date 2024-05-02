using System.Globalization;
using System.Net;
using System.Text.Json;
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
    private readonly Mock<ISunriseSunsetProvider> _sunriseSunsesProviderMock;
    
    public SunriseSunsetControllerTest(ITestOutputHelper outputHelper)
    {
        _factory = new CustomWebApplicationFactory();
        _coordinateDataProviderMock = _factory.CoordinateDataProviderMock;
        _jsonProcessorMock = _factory.JsonProcessorMock;
        _sunriseSunsesProviderMock = _factory.SunriseSunsetProviderMock;
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
    public async Task GetSunriseSunsetReturnsNotFoundResultIfCityNotFoundInDbAndInExternalApi()
    {
        // Arrange
        _coordinateDataProviderMock.Setup(x => x.GetCityFromOpenWeatherMap(It.IsAny<string>()))
            .Throws(new JsonException("Failed to get city from API"));
        // Act
        var response = await _httpClient.GetAsync("/api/SunriseSunset/GetSunriseSunset?cityName=fgnfghjghjfgvhbnbfghvnhj");
        var responseContent = await response.Content.ReadAsStringAsync();
        _outputHelper.WriteLine(responseContent);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
    
    [Fact]
    public async Task GetSunriseSunsetReturnsBadRequestResultIfDateFormatIsNotValid()
    {
        // Arrange
        var city = "Budapest";
        var invalidDateTime = "invalidDateTime";
        
        // Set up mock behavior
        _coordinateDataProviderMock.Setup(x => x.GetCityFromOpenWeatherMap(city))
            .Throws(new FormatException("Time format is not valid."));

        // Act
        var response = await _httpClient.GetAsync($"/api/sunrisesunset/GetSunriseSunset?cityName={city}&date={invalidDateTime}");
        var responseContent = await response.Content.ReadAsStringAsync();
        _outputHelper.WriteLine(responseContent);

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
    
    [Fact]
    public async Task GetSunriseSunsetReturnsOkIfCityIsNotInDbAndSunriseSunsetIsAlsoNotInDb()
    {
        // Arrange
        var city = "C";
        var datetime = "2024-05-02";
        City createdCity = new City
        {
            Country = "DE",
            Longitude = 0,
            Latitude = 0,
            Name = "TestCity",
            State = "G"
        };
        
        SunriseSunsetOfCity expectedResult = new SunriseSunsetOfCity
        {
            City = createdCity,
            Date = DateTime.Today,
            Sunrise = "sunrise",
            Sunset = "sunset",
            TimeZone = "UTC"
        };
        
        _coordinateDataProviderMock.Setup(x => x.GetCityFromOpenWeatherMap(It.IsAny<string>()))
            .ReturnsAsync(city);
        _jsonProcessorMock.Setup(x => x.ProcessWeatherApiCityStringToCity(city))
            .ReturnsAsync(createdCity);
        _sunriseSunsesProviderMock.Setup(x => x.GetSunriseSunset(createdCity.Latitude, createdCity.Longitude, It.IsAny<string>()))
            .ReturnsAsync(datetime);
        _jsonProcessorMock.Setup(x =>
                x.ProcessSunriseSunsetApiStringToSunriseSunset(createdCity, DateTime.Today, datetime))
            .Returns(expectedResult);
        
        // Act
        var response = await _httpClient.GetAsync($"/api/Sunrisesunset/GetSunriseSunset?cityName={city}&date={datetime}");

        // Assert
        response.EnsureSuccessStatusCode(); // Status Code 200-299
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var responseContent = await response.Content.ReadAsStringAsync();
        _outputHelper.WriteLine(responseContent);
        Assert.Contains("TestCity", responseContent);
        Assert.Contains("sunset", responseContent);
    }
    

    
    
    
}