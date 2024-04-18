using Microsoft.AspNetCore.Mvc;
using SolarWatch.Services.Repository;

namespace SolarWatch.Controllers;

[ApiController]
[Route("[controller]")]
public class CityController : ControllerBase
{
    private readonly ILogger<SunriseSunsetController> _logger;
    private readonly ICityRepository _cityRepository;

    public CityController(ILogger<SunriseSunsetController> logger, ICityRepository cityRepository)
    {
        _logger = logger;
        _cityRepository = cityRepository;
    }
    
    [HttpGet("Cities")]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var cities = await _cityRepository.GetAll();
            return Ok(cities);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error occured while trying to list all cities.");
            return StatusCode(500, "An error occured while trying to list all cities.");
        }
    }
    
    //Not yet finished
    [HttpGet("{country}/{city}")]
    public async Task<IActionResult> GetCity()
    {
        try
        {
            var cities = await Task.Run(() => _cityRepository.GetAll());
            return Ok(cities);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error occured while trying to list all cities.");
            return StatusCode(500, "An error occured while trying to list all cities.");
        }
    }
}