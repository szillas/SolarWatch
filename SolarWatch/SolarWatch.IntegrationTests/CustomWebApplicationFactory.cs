using System.Data.Common;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SolarWatch.Data;
using SolarWatch.Data.SeedData;

namespace SolarWatch.IntegrationTests;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            var dbContext = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<UsersContext>));

            if (dbContext != null)
                services.Remove(dbContext);

            var serviceProvider = new ServiceCollection().AddEntityFrameworkInMemoryDatabase().BuildServiceProvider();

            services.AddDbContext<UsersContext>(options =>
            {
                options.UseInMemoryDatabase("InMemoryAuthTest");
                options.UseInternalServiceProvider(serviceProvider);
            });
            var sp = services.BuildServiceProvider();

            using var scope = sp.CreateScope();
            using var appContext = scope.ServiceProvider.GetRequiredService<UsersContext>();
            appContext.Database.EnsureCreated();
            
        });
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