using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using SolarWatch.Data;
using SolarWatch.Services.Authentication;
using SolarWatch.Services.JsonProcessor;
using SolarWatch.Services.Providers.CoordinateProvider;
using SolarWatch.Services.Providers.SunriseSunsetProvider;
using SolarWatch.Services.Repository;

var builder = WebApplication.CreateBuilder(args);

var issuerKey = builder.Configuration["SolarWatch:SecretKey"];
var sqlServerConnectionString = builder.Configuration.GetConnectionString("SqlServerDefault");

// Add services to the container.
AddServices();
ConfigureSwagger();
AddDbContext();
AddAuthentication();
AddIdentity();

var app = builder.Build();

ApplyMigrations();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseDeveloperExceptionPage();
    app.UseCors("CorsPolicy");
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();


void AddServices()
{
    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSingleton<ICoordinateDataProvider, OpenWeatherCoordDataProviderApi>();
    builder.Services.AddSingleton<ISunriseSunsetProvider, SunriseSunsetApi>();
    builder.Services.AddSingleton<IJsonProcessor, JsonProcessor>();
    builder.Services.AddScoped<ICityRepository, CityRepository>();
    builder.Services.AddScoped<ISunriseSunsetRepository, SunriseSunsetRepository>();
    builder.Services.AddScoped<IAuthService, AuthService>();
    builder.Services.AddScoped<ITokenService, TokenService>();
}

void ConfigureSwagger()
{
    builder.Services.AddSwaggerGen(option =>
    {
        option.SwaggerDoc("v1", new OpenApiInfo { Title = "Demo API", Version = "v1" });
        option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            In = ParameterLocation.Header,
            Description = "Please enter a valid token",
            Name = "Authorization",
            Type = SecuritySchemeType.Http,
            BearerFormat = "JWT",
            Scheme = "Bearer"
        });
        option.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type=ReferenceType.SecurityScheme,
                        Id="Bearer"
                    }
                },
                new string[]{}
            }
        });
    });
}

void AddDbContext()
{
    builder.Services.AddDbContext<SolarWatchApiContext>(options => 
        options.UseSqlServer(sqlServerConnectionString));
    builder.Services.AddDbContext<UsersContext>(options => 
        options.UseSqlServer(sqlServerConnectionString));
}

void AddAuthentication()
{
    builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters()
            {
                ClockSkew = TimeSpan.Zero,
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = builder.Configuration["Jwt:Issuer"],
                ValidAudience = builder.Configuration["Jwt:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(issuerKey ?? throw new InvalidOperationException())
                ),
            };
        });
}

void AddIdentity()
{
    builder.Services
        .AddIdentityCore<IdentityUser>(options =>
        {
            options.SignIn.RequireConfirmedAccount = false;
            options.User.RequireUniqueEmail = true;
            options.Password.RequireDigit = false;
            options.Password.RequiredLength = 6;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequireUppercase = false;
            options.Password.RequireLowercase = false;
        })
        .AddRoles<IdentityRole>()
        .AddEntityFrameworkStores<UsersContext>();
}

async void ApplyMigrations()
{
    var isTestEnvironment = Environment.GetEnvironmentVariable("IS_TEST_ENVIRONMENT");

    if (string.IsNullOrEmpty(isTestEnvironment) || !bool.TryParse(isTestEnvironment, out var isTest) || !isTest)
    {
        using var scope = app.Services.CreateScope();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

        const int maxRetries = 6;
        int retries = 0;
        bool dbReady = false;

        while (!dbReady && retries < maxRetries)
        {
            try
            {
                var usersContext = scope.ServiceProvider.GetRequiredService<UsersContext>();
                var solarWatchApiContext = scope.ServiceProvider.GetRequiredService<SolarWatchApiContext>();

                await usersContext.Database.CanConnectAsync();
                await solarWatchApiContext.Database.CanConnectAsync();

                usersContext.Database.Migrate();
                solarWatchApiContext.Database.Migrate();

                dbReady = true;
                logger.LogInformation("Migrations applied successfully.");
            }
            catch (Exception ex)
            {
                retries++;
                logger.LogWarning(ex,
                    "Database not ready. Waiting before retrying... Attempt {Attempt} of {MaxRetries}", retries,
                    maxRetries);
                await Task.Delay(TimeSpan.FromSeconds(Math.Pow(2, retries))); // Exponential backoff
            }
        }

        if (!dbReady)
        {
            logger.LogError("Failed to apply migrations after {MaxRetries} attempts.", maxRetries);
        }
    }
}


public partial class Program { }