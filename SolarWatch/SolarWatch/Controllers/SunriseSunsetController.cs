﻿using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using SolarWatch.CoordinateProvider;
using SolarWatch.JsonProcessor;
using SolarWatch.Model;
using SolarWatch.Services.Repository;
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
    
    [HttpGet("GetSunriseSunset")]
    public async Task<ActionResult<SunriseSunset>> GetSunriseSunset(string cityName, string? date)
    {
        try
        {
            var city = _cityRepository.GetByName(cityName);
            if (city == null)
            {
                var openWeatherMapData = await _coordinateDataProvider.GetCityFromOpenWeatherMap(cityName);
                city = _jsonProcessor.ProcessWeatherApiCityStringToCity(openWeatherMapData);
                _cityRepository.Add(city);
            }
            
            Coordinate coordinate = new Coordinate
            {
                Longitude = city.Longitude, Latitude = city.Latitude
            };
            _logger.LogInformation(coordinate.ToString());

            var sunriseSunset = _sunriseSunsetRepository.GetByDateAndCity(cityName, date);
            if (sunriseSunset == null)
            {
                var sunriseSunsetData = await _sunriseSunsetProvider.GetSunriseSunset(coordinate, date!);
                sunriseSunset =
                    _jsonProcessor.ProcessSunriseSunsetApiStringToSunriseSunset(city, date, sunriseSunsetData);
                _sunriseSunsetRepository.Add(sunriseSunset);
            }
            
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
}