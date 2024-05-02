using System.Net;
using System.Net.Http.Json;
using System.Runtime.InteropServices.JavaScript;
using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using SolarWatch.Contracts;
using SolarWatch.Data;
using SolarWatch.Services.Providers.CoordinateProvider;
using Xunit.Abstractions;

namespace SolarWatch.IntegrationTests;

[Collection("Integration Tests")]
public class AuthControllerTest : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _httpClient;
    private readonly ITestOutputHelper _outputHelper;
    private readonly CustomWebApplicationFactory _factory;

    public AuthControllerTest(ITestOutputHelper outputHelper)
    {
        _factory = new CustomWebApplicationFactory();
        _httpClient = _factory.CreateClient();
        _outputHelper = outputHelper;
    }
    
    
    [Fact]
    public void SeededDataIsPresentInDatabase()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<UsersContext>();

        // Act 
        var users = dbContext.Users.ToList();
        foreach (var user in users)
        {
            _outputHelper.WriteLine(user.UserName);
        }

        // Assert
        Assert.NotNull(users);
        Assert.NotEmpty(users); 
    }
    
    [Fact]
    public async Task Register_ReturnsSuccessStatusCode()
    {
        // Arrange
        var request = new RegistrationRequest("t@t.com", "User1", "Password");

        // Act
        var jsonContent = JsonSerializer.Serialize(request);
        var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync("/api/auth/Register", content);

        // Assert
        response.EnsureSuccessStatusCode(); // Status Code 200-299
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        
        if (response.StatusCode == HttpStatusCode.Created)
        {
           
            var responseContent = await response.Content.ReadAsStringAsync();
            _outputHelper.WriteLine(responseContent);
            
            // Customize JSON serialization options
            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase // Use camelCase property naming convention
            };
            var registrationResponse = JsonSerializer.Deserialize<RegistrationResponse>(responseContent, options);
            _outputHelper.WriteLine(registrationResponse.ToString());
            
            Assert.NotNull(registrationResponse);
            Assert.Equal("t@t.com", registrationResponse.Email);
            Assert.Equal(request.Email, registrationResponse.Email);
            Assert.Equal(request.UserName, registrationResponse.UserName);
        }
    }
    
    [Fact]
    public async Task Register_WithInvalidData_ReturnsBadRequest()
    {
        // Arrange
        var invalidRequest = new RegistrationRequest("", "", "");

        // Act
        var jsonContent = JsonSerializer.Serialize(invalidRequest);
        var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync("/api/auth/Register", content);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        
        var responseContent = await response.Content.ReadAsStringAsync();
        _outputHelper.WriteLine(responseContent);
    }
    
    [Fact]
    public async Task Login_ReturnsSuccessStatusCode()
    {
        // Register a user
        var registerRequest = new RegistrationRequest("t@t.com", "User1", "Password");
        var registerJsonContent = JsonSerializer.Serialize(registerRequest);
        var registerContent = new StringContent(registerJsonContent, Encoding.UTF8, "application/json");
        var registerResponse = await _httpClient.PostAsync("/api/auth/Register", registerContent);
        registerResponse.EnsureSuccessStatusCode();
        
        // Login with the registered user
        var loginRequest = new AuthRequest("t@t.com", "Password");
        var loginJsonContent = JsonSerializer.Serialize(loginRequest);
        var loginContent = new StringContent(loginJsonContent, Encoding.UTF8, "application/json");
        var loginResponse = await _httpClient.PostAsync("/api/auth/Login", loginContent);

        // Assert
        loginResponse.EnsureSuccessStatusCode(); // Status Code 200-299
        Assert.Equal(HttpStatusCode.OK, loginResponse.StatusCode);


        var responseContent = await loginResponse.Content.ReadAsStringAsync();

        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
        var authResponse = JsonSerializer.Deserialize<AuthResponse>(responseContent, options);
        _outputHelper.WriteLine(authResponse.ToString());
        
        Assert.NotNull(authResponse);
        Assert.Equal("t@t.com", authResponse.Email);
        
        string authToken = authResponse.Token;

        _outputHelper.WriteLine($"Token: {authToken}");
    }
    
    
}