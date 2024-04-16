using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SolarWatch.CoordinateProvider;
using SolarWatch.Data;
using SolarWatch.JsonProcessor;
using SolarWatch.Services.Authentication;
using SolarWatch.Services.Repository;
using SolarWatch.SunriseSunsetProvider;

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

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseDeveloperExceptionPage();
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
}

void ConfigureSwagger()
{
    builder.Services.AddSwaggerGen();
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
                    Encoding.UTF8.GetBytes(issuerKey)
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
        .AddEntityFrameworkStores<UsersContext>();
}