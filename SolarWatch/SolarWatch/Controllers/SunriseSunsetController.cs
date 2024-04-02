using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using SolarWatch.CoordinateProvider;
using SolarWatch.JsonProcessor;
using SolarWatch.Model;
using SolarWatch.SunriseSunsetProvider;

namespace SolarWatch.Controllers;

[ApiController]
[Route("[controller]")]
public class SunriseSunsetController : ControllerBase
{
    private readonly ILogger<SunriseSunsetController> _logger;
    private readonly ICoordinateDataProvider _coordinateDataProvider;
    private readonly ISunriseSunsetProvider _sunriseSunsetProvider;
    private readonly IJsonProcessor _jsonProcessor;

    public SunriseSunsetController(ILogger<SunriseSunsetController> logger, ICoordinateDataProvider coordinateDataProvider, 
        IJsonProcessor jsonProcessor, ISunriseSunsetProvider sunriseSunsetProvider)
    {
        _logger = logger;
        _coordinateDataProvider = coordinateDataProvider;
        _jsonProcessor = jsonProcessor;
        _sunriseSunsetProvider = sunriseSunsetProvider;
    }
    
    [HttpGet("GetSunriseSunset")]
    public ActionResult<SunriseSunset> GetSunriseSunset(string city, string? date)
    {
        try
        {
            var openWeatherMapData = _coordinateDataProvider.GetCoordinate(city);
            _logger.LogInformation(openWeatherMapData);
            Coordinate coordinate2 = _jsonProcessor.ProcessWeatherApiCityToCoordinate(openWeatherMapData);
            _logger.LogInformation(coordinate2.ToString());

            if (!DateTime.TryParse(date, out var dateValue) || string.IsNullOrEmpty(date))
            {
                dateValue = DateTime.Today;
                date = dateValue.ToString("yyyy-MM-dd");
            }
            
            var sunriseSunsetData = _sunriseSunsetProvider.GetSunriseSunset(coordinate2.Latitude, coordinate2.Longitude, date);
            return Ok(_jsonProcessor.ProcessSunriseSunsetApi(city, dateValue, sunriseSunsetData));
            
        }
        catch (JsonException e)
        {
            _logger.LogError(e, "Error processing API call");
            return BadRequest(e.Message);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error getting sunrise/sunset");
            return NotFound("Error getting sunrise/sunset");
        }
    }
}