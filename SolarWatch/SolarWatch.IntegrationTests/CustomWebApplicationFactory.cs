using System.Data.Common;
using System.Globalization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using SolarWatch.Contracts;
using SolarWatch.Data;
using SolarWatch.Data.SeedData;
using SolarWatch.Model;

namespace SolarWatch.IntegrationTests;

//Used to create instances of the web application for testing
public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            //This line retrieves the DbContextOptions for the UsersContext from the collection of services. It checks
            //if such a service is already registered.
            var dbContext = services
                .SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<UsersContext>));
            if (dbContext != null)
                services.Remove(dbContext);
            
            var solarWatchDbContextOptions = services
                .SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<SolarWatchApiContext>));
            if (solarWatchDbContextOptions != null)
            {
                services.Remove(solarWatchDbContextOptions);
            }
            
            //This line creates a new ServiceProvider by configuring an in-memory database provider for Entity
            //Framework. This is used for dependency injection during testing.
            var serviceProvider = new ServiceCollection().AddEntityFrameworkInMemoryDatabase().BuildServiceProvider();

            //Here, the UsersContext is added to the services collection with AddDbContext. It configures the context
            //to use an in-memory database named "InMemoryAuthTest" and specifies the internal service provider created earlier.
            //This line creates a new instance of ServiceCollection, configures it to use the Entity Framework in-memory
            //database provider , and then builds a ServiceProvider from it (BuildServiceProvider()).
            //This ServiceProvider is typically used internally within the test setup for dependency injection purposes,
            //such as providing a database context for testing.
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
            
            //The services collection is built into a ServiceProvider. This allows resolving services from the collection.
            //This line builds a ServiceProvider from the existing ServiceCollection named services. This ServiceCollection
            //was provided as an argument to the ConfigureServices method within the ConfigureWebHost method of the
            //WebApplicationFactory. It is used by the ASP.NET Core application to configure services during startup.
            var sp = services.BuildServiceProvider();

            //A scoped service provider is created to manage the scope of service lifetimes. Then, an instance of the
            //UsersContext is retrieved from the scoped service provider.
            using var scope = sp.CreateScope();
            using var usersContext = scope.ServiceProvider.GetRequiredService<UsersContext>();
            usersContext.Database.EnsureCreated();
            
            using var solarWatchDbContext = scope.ServiceProvider.GetRequiredService<SolarWatchApiContext>();
            solarWatchDbContext.Database.EnsureCreated();
            
            //SeedUsersContext(usersContext);
            
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
    
    
    
}



/*
 builder.ConfigureTestServices(services =>
    {
        var dbContextDescriptor =
            services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<UsersContext>));
        services.Remove(dbContextDescriptor);
        var dbConnectionDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbConnection));
        services.Remove(dbConnectionDescriptor);

        services.AddSingleton<DbConnection>(container =>
        {
            var connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();

            return connection;
        });

        services.AddDbContext<UsersContext>((container, options) =>
        {
            var connection = container.GetRequiredService<DbConnection>();
            options.UseSqlite(connection);
        });

        // Seed authentication data
        SeedAuthenticationData(services.BuildServiceProvider());
    });
 */
   /* private static void SeedAuthenticationData(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var scopedServices = scope.ServiceProvider;
        var context = scopedServices.GetRequiredService<UsersContext>();

        try
        {
            // Ensure the database is created and migrated
            context.Database.EnsureCreated();

            // Run the authentication seeder
            var seeder = new AuthenticationSeeder(
                scopedServices.GetRequiredService<RoleManager<IdentityRole>>(),
                scopedServices.GetRequiredService<UserManager<IdentityUser>>(),
                scopedServices.GetRequiredService<IConfiguration>()
            );
            seeder.AddRoles(); // Seed roles
            seeder.AddAdmin(); // Seed admin user
        }
        catch (Exception ex)
        {
            // Log any exceptions if needed
            // Handle exceptions as appropriate
            Console.WriteLine($"An error occurred while seeding the authentication data: {ex.Message}");
        }
    }
}*/


/*builder.UseEnvironment("Test");
        builder.ConfigureServices(services =>
        {
            var context = services.SingleOrDefault(descriptor => descriptor.ServiceType == typeof(DbContextOptions<UsersContext>));
            /*if (context != null)
            {
                services.Remove(context);
                var options = services
                    .Where(r => r.ServiceType == typeof(DbContextOptions) ||
                                r.ServiceType.IsGenericType && r.ServiceType.GetGenericTypeDefinition() ==
                                typeof(DbContextOptions<>)).ToArray();
                foreach (var option in options)
                {
                    services.Remove(option);
                }
            }
if (context != null)
{
    services.Remove(context);
}    
            
// Add a new registration for ApplicationDbContext with an in-memory database
services.AddDbContext<UsersContext>(options =>
{
    // Provide a unique name for your in-memory database
    options.UseInMemoryDatabase("InMemoryDatabaseForTest");
});
});*/