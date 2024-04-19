using Microsoft.EntityFrameworkCore;
using SolarWatch.Data;
using SolarWatch.Model;

namespace SolarWatch.Services.Repository;

public class SunriseSunsetRepository : ISunriseSunsetRepository
{
    private readonly SolarWatchApiContext _dbContext;

    public SunriseSunsetRepository(SolarWatchApiContext context)
    {
        _dbContext = context;
    }

    public async Task<IEnumerable<SunriseSunsetOfCity>> GetAll()
    {
        return await _dbContext.SunriseSunsetOfCities.ToListAsync();
    }
    
    public async Task<SunriseSunsetOfCity?> GetById(int id)
    {
        return await _dbContext.SunriseSunsetOfCities.FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<SunriseSunsetOfCity?> GetByDateAndCity(string city, DateTime date)
    {
        var sunriseSunset = await _dbContext.SunriseSunsetOfCities
            .Include(s => s.City)
            .FirstOrDefaultAsync(s => s.City.Name == city && s.Date == date);

        return sunriseSunset;
    }
    
    public async Task<SunriseSunsetOfCity?> GetByDateAndCity(string city, string? date)
    {
        var parsedDate = DateParser(date);
        var sunriseSunset = await _dbContext.SunriseSunsetOfCities
            .Include(ss => ss.City) 
            .FirstOrDefaultAsync(ss => ss.City.Name == city && ss.Date == parsedDate);

        return sunriseSunset;
    }

    public async Task Add(SunriseSunsetOfCity sunriseSunset)
    {
        _dbContext.Add(sunriseSunset);
        await _dbContext.SaveChangesAsync();
    }

    public async Task Delete(SunriseSunsetOfCity sunriseSunset)
    {
        _dbContext.Remove(sunriseSunset);
        await _dbContext.SaveChangesAsync();
    }

    public async Task Update(SunriseSunsetOfCity sunriseSunset)
    {  
        var sunriseSunsetInDb = await _dbContext.SunriseSunsetOfCities.FindAsync(sunriseSunset.Id);
        
        if (sunriseSunsetInDb == null)
        {
            throw new Exception($"City with ID {sunriseSunset.Id} not found.");
        }

        sunriseSunsetInDb.Date = sunriseSunset.Date;
        sunriseSunsetInDb.TimeZone = sunriseSunset.TimeZone;
        sunriseSunsetInDb.Sunrise = sunriseSunset.Sunrise;
        sunriseSunsetInDb.Sunset = sunriseSunset.Sunset;

        _dbContext.Update(sunriseSunsetInDb);
        await _dbContext.SaveChangesAsync();
    }
    
    private static DateTime DateParser(string? date)
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