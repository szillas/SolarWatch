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
    
    [HttpGet("GetSunriseSunset2")]
    public ActionResult<SunriseSunset> GetSunriseSunset2(string city, string? date)
    {
        try
        {
            var openWeatherMapData = _coordinateDataProvider.GetCoordinate(city);
            //ActionResult<Coordinate> coordinateResult = Ok(_jsonProcessor.ProcessWeatherApiCityToCoordinate(openWeatherMapData));
            Coordinate coordinate2 = _jsonProcessor.ProcessWeatherApiCityToCoordinate(openWeatherMapData);
            _logger.LogInformation(coordinate2.Latitude.ToString());

            DateTime dateValue;
            if (!DateTime.TryParse(date, out dateValue) || string.IsNullOrEmpty(date))
            {
                dateValue = DateTime.Today;
                date = dateValue.ToString("yyyy-MM-dd");
            }
            
            var sunriseSunsetData = _sunriseSunsetProvider.GetSunriseSunset(coordinate2.Latitude, coordinate2.Longitude, date);
            return Ok(_jsonProcessor.ProcessSunriseSunsetApi(city, dateValue, sunriseSunsetData));
            
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error getting sunrise/sunset");
            return NotFound("Error getting sunrise/sunset");
        }
    }

    [HttpGet("GetSunriseSunset")]
    public ActionResult<SunriseSunset> GetSunriseSunset(string city, string? date)
    {
        //string city = "Budapest";

        try
        {
            ActionResult<Coordinate> coordinateResult = GetCoordinate(city);
            
            /* Using if statement to convert result to coordinate*/
            Coordinate? coordinate2;
            if (coordinateResult.Result is OkObjectResult okObjectResult && okObjectResult.Value is Coordinate)
            {
                coordinate2 = okObjectResult.Value as Coordinate;
                _logger.LogInformation("It is working!");
            }
            else
            {
                coordinate2 = new Coordinate{Longitude = 0, Latitude = 0};
            }
            /* ------------------------------------------------------ */
            
            /*Using OkObjectResultCasting*/
            var result = (OkObjectResult)coordinateResult.Result;

            _logger.LogInformation("Logging");
            Coordinate coordinate = result.Value as Coordinate;
            if (coordinate != null)
            {
                _logger.LogInformation(coordinate.Longitude.ToString());
            }
            /*  ----------------------------------------------------- */

            DateTime.TryParse(date, out var dateValue);
            if (date == null)
            {
                dateValue = DateTime.Today;
                date = dateValue.ToString("yyyy-MM-dd");
            }
            
            var sunriseSunsetData = _sunriseSunsetProvider.GetSunriseSunset(coordinate2.Latitude, coordinate2.Longitude, date);
            return Ok(_jsonProcessor.ProcessSunriseSunsetApi(city, dateValue, sunriseSunsetData));
            
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error getting sunrise/sunset");
            return NotFound("Error getting sunrise/sunset");
        }
    }
    
    [HttpGet("GetCoordinate")]
    public ActionResult<Coordinate> GetCoordinate(string city)
    {

        try
        {
            var openWeatherMapData = _coordinateDataProvider.GetCoordinate(city);
            return Ok(_jsonProcessor.ProcessWeatherApiCityToCoordinate(openWeatherMapData));
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error getting coordinate");
            return NotFound("Error getting coordinate");
        }
    }
}