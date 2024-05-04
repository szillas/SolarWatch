using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SolarWatch.Contracts.SunriseSunset;
using SolarWatch.Model;
using SolarWatch.Model.NotInUse;
using SolarWatch.Services.Extensions;
using SolarWatch.Services.JsonProcessor;
using SolarWatch.Services.Providers.CoordinateProvider;
using SolarWatch.Services.Providers.SunriseSunsetProvider;
using SolarWatch.Services.Repository;

namespace SolarWatch.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SunriseSunsetController : ControllerBase
{
    private readonly ILogger<SunriseSunsetController> _logger;
    private readonly ICoordinateDataProvider _coordinateDataProvider;
    private readonly ISunriseSunsetProvider _sunriseSunsetProvider;
    private readonly IJsonProcessor _jsonProcessor;
    private readonly ICityRepository _cityRepository;
    private readonly ISunriseSunsetRepository _sunriseSunsetRepository;

    public SunriseSunsetController(ILogger<SunriseSunsetController> logger, ICoordinateDataProvider coordinateDataProvider, 
        IJsonProcessor jsonProcessor, ISunriseSunsetProvider sunriseSunsetProvider, ICityRepository cityRepository,
        ISunriseSunsetRepository sunriseSunsetRepository)
    {
        _logger = logger;
        _coordinateDataProvider = coordinateDataProvider;
        _jsonProcessor = jsonProcessor;
        _sunriseSunsetProvider = sunriseSunsetProvider;
        _cityRepository = cityRepository;
        _sunriseSunsetRepository = sunriseSunsetRepository;
    }
    
    [Authorize]
    [HttpGet("GetSunriseSunset")]
    public async Task<ActionResult<SunriseSunsetOfCity>> GetSunriseSunset(string cityName, string? date)
    {
        try
        {
            var city = await GetCityFromDbOrApi(cityName);
            var dateTime = date.ParseDateOrDefaultToToday();
            var sunriseSunset = await GetSunFromDbOrApi(city, dateTime, date);
            
            return Ok(sunriseSunset);
            
        }
        catch (JsonException e)
        {
            _logger.LogError(e, "Error processing API call.");
            return BadRequest(e.Message);
        }
        catch (FormatException e)
        {
            _logger.LogError(e, "Date format is not correct.");
            return BadRequest(e.Message);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error getting sunrise/sunset.");
            return NotFound("Error getting sunrise/sunset");
        }
    }
    
    [HttpPost]
    public async Task<IActionResult> AddSunriseSunset(SunriseSunsetDto sunriseSunsetDto)
    {
        var city = await _cityRepository.GetByNameAndCountry(sunriseSunsetDto.CityName, sunriseSunsetDto.Country);
        if (city == null)
        {
            return BadRequest($"City with name '{sunriseSunsetDto.CityName}' and country '{sunriseSunsetDto.Country}' not found.");
        }
        
        var sunriseSunset = new SunriseSunsetOfCity
        {
            Date = sunriseSunsetDto.Date,
            Sunrise = sunriseSunsetDto.Sunrise,
            Sunset = sunriseSunsetDto.Sunset,
            TimeZone = sunriseSunsetDto.TimeZone,
            City = city
        };
        
        try
        {
            await _sunriseSunsetRepository.Add(sunriseSunset);
            return CreatedAtAction(nameof(GetSunriseSunset), new { id = sunriseSunset.Id }, sunriseSunset);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error occured while trying to add sunrise/sunset data.");
            return StatusCode(500, "An error occured while trying to add sunrise/sunset data.");
        }
    }
    
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteSunriseSunset(int id)
    {
        try
        {
            var sunriseSunsetInDb = await _sunriseSunsetRepository.GetById(id);
            if (sunriseSunsetInDb == null)
            {
                return NotFound($"Sunrise/sunset with id {id} does not exist in the db.");
            }
            await _sunriseSunsetRepository.Delete(sunriseSunsetInDb);
            return NoContent();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error occured while trying to delete Sunrise/Sunset from Db.");
            return StatusCode(500, "An error occured while trying to delete Sunrise/Sunset from Db.");
        }
    }
    
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateSunriseSunset(int id, SunriseSunsetOfCity sunriseSunset)
    {
        if (id != sunriseSunset.Id)
        {
            return BadRequest("ID in the URL does not match the ID in the request body.");
        }
        try
        {
            await _sunriseSunsetRepository.Update(sunriseSunset);
            return Ok(sunriseSunset); 
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error occured while trying to modify sunrise/sunset with id {id}.", id);
            return StatusCode(500, $"An error occured while trying to modify sunrise/sunset with id {id}.");
        }
    }

    private async Task<City> GetCityFromDbOrApi(string cityName)
    {
        var city = await _cityRepository.GetByName(cityName);
        if (city == null)
        {
            var openWeatherMapData = await _coordinateDataProvider.GetCityFromOpenWeatherMap(cityName);
            city = await _jsonProcessor.ProcessWeatherApiCityStringToCity(openWeatherMapData);
            await _cityRepository.Add(city);
        }
        return city;
    }
    
    //Working on this
    private async Task<City> GetCityFromDbOrApiWithCountryCode(string cityName, string? country, string? state)
    {
        City? city;

        if (country == null && state == null)
        {
            city = await _cityRepository.GetByName(cityName);
        }
        else if ((country == "US" || country == "USA") && state != null)
        {
            city = await _cityRepository.GetByName(cityName, state);
        }
        else if (country != null)
        {
            city = await _cityRepository.GetByNameAndCountry(cityName, country);
        }
        else
        {
            throw new ArgumentException("Invalid country or state provided.");
        }
        
        /*var city = await _cityRepository.GetByNameAndCountry(cityName, country);
        var city2 = await _cityRepository.GetByName(cityName, state);*/
        if (city == null)
        {
            var openWeatherMapData = await _coordinateDataProvider.GetCityFromOpenWeatherMap(cityName, country);
            city = await _jsonProcessor.ProcessWeatherApiCityStringToCity(openWeatherMapData);
            await _cityRepository.Add(city);
        }
        return city;
    }

    private async Task<SunriseSunsetOfCity> GetSunFromDbOrApi(City city, DateTime dateTime, string? date)
    {
        var sunriseSunset = await _sunriseSunsetRepository.GetByDateAndCity(city.Name, dateTime);
        if (sunriseSunset == null)
        {
            var sunriseSunsetData = await _sunriseSunsetProvider.GetSunriseSunset(city.Latitude, city.Longitude, date);
            sunriseSunset = _jsonProcessor.ProcessSunriseSunsetApiStringToSunriseSunset(city, dateTime, sunriseSunsetData);
            await _sunriseSunsetRepository.Add(sunriseSunset);
        }
        return sunriseSunset;
    }
}