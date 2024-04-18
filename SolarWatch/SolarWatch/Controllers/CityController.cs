using Microsoft.AspNetCore.Mvc;
using SolarWatch.Model;
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
    
    [HttpGet("{country}/{city}")]
    public async Task<IActionResult> GetCity(string country, string city)
    {
        try
        {
            var cityInRepo = await _cityRepository.GetByNameAndCountry(city, country);
            if (cityInRepo == null)
            {
                return NotFound("City is not found!");
            }
            return Ok(cityInRepo);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error occured while trying to get the city.");
            return StatusCode(500, "An error occured while trying to get the city.");
        }
    }
    
    [HttpPost]
    public async Task<IActionResult> AddCity(City city)
    {
        try
        {
            await _cityRepository.Add(city);
            return CreatedAtAction(nameof(GetCity), new { country = city.Country, city = city.Name }, city);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error occured while trying to add a city.");
            return StatusCode(500, "An error occured while trying to add a city.");
        }
    }
}