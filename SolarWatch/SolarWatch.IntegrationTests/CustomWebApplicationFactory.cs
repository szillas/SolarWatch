using System.Globalization;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using SolarWatch.Data;
using SolarWatch.Model;
using SolarWatch.Services.Providers.CoordinateProvider;
using SolarWatch.Services.Providers.SunriseSunsetProvider;

namespace SolarWatch.IntegrationTests;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    public Mock<ICoordinateDataProvider> CoordinateDataProviderMock { get; } = new();
    public Mock<ISunriseSunsetProvider> SunriseSunsetProviderMock { get; } = new();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            Environment.SetEnvironmentVariable("IS_TEST_ENVIRONMENT", "true");
            
            var usersDbContext = services
                .SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<UsersContext>));
            if (usersDbContext != null)
                services.Remove(usersDbContext);
            
            var solarWatchDbContext = services
                .SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<SolarWatchApiContext>));
            if (solarWatchDbContext != null)
                services.Remove(solarWatchDbContext);
            
            var coordinateDataProvider = services
                .SingleOrDefault(d => d.ServiceType == typeof(ICoordinateDataProvider));
            if (coordinateDataProvider != null)
                services.Remove(coordinateDataProvider);
            
            var sunriseSunsetProvider = services
                .SingleOrDefault(d => d.ServiceType == typeof(ISunriseSunsetProvider));
            if (sunriseSunsetProvider != null)
                services.Remove(sunriseSunsetProvider);
            
            services.AddSingleton<ICoordinateDataProvider>(_=> CoordinateDataProviderMock.Object);
            services.AddSingleton<ISunriseSunsetProvider>(_ => SunriseSunsetProviderMock.Object);
            
            //Remove comment from next line to use a Fake Policy evaluator!
            //services.AddSingleton<IPolicyEvaluator, FakePolicyEvaluator>();
            
            var serviceProvider = new ServiceCollection().AddEntityFrameworkInMemoryDatabase().BuildServiceProvider();

            services.AddDbContext<UsersContext>(options =>
            {
                options.UseInMemoryDatabase("InMemoryTest");
                options.UseInternalServiceProvider(serviceProvider);
            });
            
            services.AddDbContext<SolarWatchApiContext>(options =>
            {
                options.UseInMemoryDatabase("InMemoryTest");
                options.UseInternalServiceProvider(serviceProvider);
            });
            
            var sp = services.BuildServiceProvider();
            
            using var scope = sp.CreateScope();
            using var usersContext = scope.ServiceProvider.GetRequiredService<UsersContext>();
            usersContext.Database.EnsureCreated();
            
            using var solarWatchContext = scope.ServiceProvider.GetRequiredService<SolarWatchApiContext>();
            solarWatchContext.Database.EnsureCreated();
            
            SeedUsersContext(usersContext);
            SeedSolarWatchApiContext(solarWatchContext);
        });
    }
    
    private void SeedUsersContext(UsersContext context)
    {
        // Create roles if they don't exist
        var roleManager = context.GetService<RoleManager<IdentityRole>>();
        var configuration = context.GetService<IConfiguration>();
        var adminRoleName = configuration?["RoleNames:Admin"];
        var userRoleName = configuration?["RoleNames:User"];

        if (roleManager != null && adminRoleName != null && userRoleName != null)
        {
            if (!roleManager.Roles.Any(r => r.Name == adminRoleName))
            {
                roleManager.CreateAsync(new IdentityRole(adminRoleName)).Wait();
            }

            if (!roleManager.Roles.Any(r => r.Name == userRoleName))
            {
                roleManager.CreateAsync(new IdentityRole(userRoleName)).Wait();
            }
        }

        // Create admin user if it doesn't exist
        var userManager = context.GetService<UserManager<IdentityUser>>();
        var adminEmail = "admin@admin.com";

        if (userManager != null && adminRoleName != null)
        {
            if (userManager.FindByEmailAsync(adminEmail).Result == null)
            {
                var adminUser = new IdentityUser
                {
                    UserName = "admin",
                    Email = adminEmail
                };

                var result = userManager.CreateAsync(adminUser, "admin123").Result;
                if (result.Succeeded)
                {
                    userManager.AddToRoleAsync(adminUser, adminRoleName).Wait();
                }
            }
        }
    }

    private void SeedSolarWatchApiContext(SolarWatchApiContext solarWatchDbContext)
    {
        City initCity = new City
        {
            Name = "London", Latitude = 51.5074, Longitude = 0.1278, Country = "UK"
        };
        solarWatchDbContext.Cities.Add(initCity);
        solarWatchDbContext.SaveChanges();
        
        var datetime = DateTime.ParseExact("2024-05-01", "yyyy-MM-dd", CultureInfo.InvariantCulture);

        solarWatchDbContext.SunriseSunsetOfCities.Add(new SunriseSunsetOfCity
        {
            City = initCity,
            Date = datetime,
            Sunrise = "00:00:00",
            Sunset = "20:00:00",
            TimeZone = "UTC"
        });
        solarWatchDbContext.SaveChanges();
    }
}