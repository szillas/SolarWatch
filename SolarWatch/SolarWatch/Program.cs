using SolarWatch.CoordinateProvider;
using SolarWatch.Data;
using SolarWatch.JsonProcessor;
using SolarWatch.Services.Repository;
using SolarWatch.SunriseSunsetProvider;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<ICoordinateDataProvider, OpenWeatherCoordDataProviderApi>();
builder.Services.AddSingleton<ISunriseSunsetProvider, SunriseSunsetApi>();
builder.Services.AddSingleton<IJsonProcessor, JsonProcessor>();
builder.Services.AddDbContext<SolarWatchApiContext>();
builder.Services.AddScoped<ICityRepository, CityRepository>();
builder.Services.AddScoped<ISunriseSunsetRepository, SunriseSunsetRepository>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();