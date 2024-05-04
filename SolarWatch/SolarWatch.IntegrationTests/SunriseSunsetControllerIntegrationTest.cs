using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Moq;
using SolarWatch.Contracts;
using SolarWatch.Data;
using SolarWatch.Services.Providers.CoordinateProvider;
using SolarWatch.Services.Providers.SunriseSunsetProvider;
using Xunit.Abstractions;

namespace SolarWatch.IntegrationTests;

[Collection("Integration Tests")]
public class SunriseSunsetControllerIntegrationTest : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _httpClient;
    private readonly ITestOutputHelper _outputHelper;
    private readonly CustomWebApplicationFactory _factory;
    private readonly Mock<ICoordinateDataProvider> _coordinateDataProviderMock;
    private readonly Mock<ISunriseSunsetProvider> _sunriseSunsesProviderMock;
    
    public SunriseSunsetControllerIntegrationTest(ITestOutputHelper outputHelper)
    {
        _factory = new CustomWebApplicationFactory();
        _coordinateDataProviderMock = _factory.CoordinateDataProviderMock;
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

    private async Task<string> LoginAdmin()
    {
        var loginRequest = new AuthRequest("admin@admin.com", "admin123");
        var loginJsonContent = JsonSerializer.Serialize(loginRequest);
        var loginContent = new StringContent(loginJsonContent, Encoding.UTF8, "application/json");
        var loginResponse = await _httpClient.PostAsync("/api/auth/Login", loginContent);
        loginResponse.EnsureSuccessStatusCode(); 
        
        var loginResponseContent = await loginResponse.Content.ReadAsStringAsync();
        
        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
        var authResponse = JsonSerializer.Deserialize<AuthResponse>(loginResponseContent, options);

        Assert.NotNull(authResponse);
        return authResponse.Token;
    }
    
    [Fact]
    public async Task GetSunriseSunsetReturnsNotFoundResultIfCityNotFoundInDbAndInExternalApi()
    {
        // Arrange
        var authToken = await LoginAdmin();
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);
        
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
        
        var authToken = await LoginAdmin();
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);
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
        var authToken = await LoginAdmin();
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);
        
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
        var city = "D";
        var datetime = DateTime.Today.ToString("yyyy-MM-dd");
        var authToken = await LoginAdmin();
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);
        
        _coordinateDataProviderMock.Setup(x => x.GetCityFromOpenWeatherMap(It.IsAny<string>()))
            .ReturnsAsync("[{\"name\":\"TestCity\",\"lat\":0.568,\"lon\":-12.698,\"country\":\"DE\",\"state\":\"G\"}]");
        _sunriseSunsesProviderMock.Setup(x => x.GetSunriseSunset(It.IsAny<double>(), It.IsAny<double>(), datetime))
            .ReturnsAsync("{\"results\":{\"sunrise\":\"00:00:00\",\"sunset\":\"22:22:22\"},\"tzid\":\"UTC\"}");
        
        // Act
        var response = await _httpClient.GetAsync($"/api/Sunrisesunset/GetSunriseSunset?cityName={city}&date={datetime}");
        var responseContent = await response.Content.ReadAsStringAsync();
        _outputHelper.WriteLine(responseContent);

        // Assert
        response.EnsureSuccessStatusCode(); // Status Code 200-299
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        Assert.Contains("TestCity", responseContent);
        Assert.Contains("sunset", responseContent);
    }

    
}