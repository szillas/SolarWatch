using Microsoft.EntityFrameworkCore;
using SolarWatch.Data;
using SolarWatch.Model;

namespace SolarWatch.Services.Repository;

public class SunriseSunsetRepository : ISunriseSunsetRepository
{
    private SolarWatchApiContext _dbContext;

    public SunriseSunsetRepository(SolarWatchApiContext context)
    {
        _dbContext = context;
    }

    public IEnumerable<SunriseSunsetOfCity> GetAll()
    {
        return _dbContext.SunriseSunsetOfCities.ToList();
    }

    public SunriseSunsetOfCity? GetByDateAndCity(int cityId, DateTime date)
    {
        return _dbContext.SunriseSunsetOfCities
            .FirstOrDefault(c => c.City.Id == cityId && c.Date == date);
    }
    
    public SunriseSunsetOfCity? GetByDateAndCity(string city, string? date)
    {
        var parsedDate = DateParser(date);
        var sunriseSunset = _dbContext.SunriseSunsetOfCities
            .Include(ss => ss.City) 
            .FirstOrDefault(ss => ss.City.Name == city && ss.Date == parsedDate);

        return sunriseSunset;
    }

    public void Add(SunriseSunsetOfCity sunriseSunset)
    {
        _dbContext.Add(sunriseSunset);
        _dbContext.SaveChanges();
    }

    public void Delete(SunriseSunsetOfCity sunriseSunset)
    {
        _dbContext.Remove(sunriseSunset);
        _dbContext.SaveChanges();
    }

    public void Update(SunriseSunsetOfCity sunriseSunset)
    {  
        _dbContext.Update(sunriseSunset);
        _dbContext.SaveChanges();
    }
    
    private DateTime DateParser(string? date)
    {
        if (string.IsNullOrEmpty(date))
        {
            return DateTime.Today;
        }
        else
        {
            if(!DateTime.TryParse(date, out var dateTime))
            {
                throw new FormatException("Date format is not correct.");
            }
            return dateTime;
        }
    }
}


/*
    public SunriseSunsetOfCity? GetByDateAndCity(string city, DateTime date)
    {
        var searchedCity = _dbContext.Cities.FirstOrDefault(c => c.Name == city);
        if (searchedCity != null)
        {
            return _dbContext.SunriseSunsetOfCities
                .FirstOrDefault(c => c.City.Id == searchedCity.Id && c.Date == date);
        }

        return null;
    }*/