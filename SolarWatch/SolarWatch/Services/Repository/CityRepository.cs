using Microsoft.EntityFrameworkCore;
using SolarWatch.Data;
using SolarWatch.Model;

namespace SolarWatch.Services.Repository;

public class CityRepository : ICityRepository
{
    private readonly SolarWatchApiContext _dbContext;

    public CityRepository(SolarWatchApiContext context)
    {
        _dbContext = context;
    }

    public async Task<IEnumerable<City>> GetAll()
    {
        return await _dbContext.Cities.ToListAsync();
    }
    public async Task<City?> GetById(int id)
    {
        return await _dbContext.Cities.FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<City?> GetByName(string name)
    {
        return await _dbContext.Cities.FirstOrDefaultAsync(c => c.Name == name);
    }
    
    public async Task<City?> GetByName(string city, string state)
    {
        return await _dbContext.Cities.FirstOrDefaultAsync(c => 
            (c.Country == "US" ||  c.Country == "USA") && c.Name == city && c.State == state);
    }
    public async Task<City?> GetByNameAndCountry(string name, string country)
    {
        return await _dbContext.Cities.FirstOrDefaultAsync(c => c.Name == name && c.Country == country);
    }

    public async Task Add(City city)
    {
        _dbContext.Add(city);
        await _dbContext.SaveChangesAsync();
    }

    public async Task Delete(City city)
    {
        _dbContext.Remove(city);
        await _dbContext.SaveChangesAsync();
    }

    public async Task Update(City city)
    {  
        var cityInDb = await _dbContext.Cities.FindAsync(city.Id);

        if (cityInDb == null)
        {
            throw new Exception($"City with ID {city.Id} not found.");
        }
        
        cityInDb.Name = city.Name;
        cityInDb.Latitude = city.Latitude;
        cityInDb.Longitude = city.Longitude;
        cityInDb.State = city.State;
        cityInDb.Country = city.Country;

        _dbContext.Update(cityInDb);
        await _dbContext.SaveChangesAsync();
    }
}