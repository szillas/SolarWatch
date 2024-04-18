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

    public async Task<City?> GetByName(string name)
    {
        return await _dbContext.Cities.FirstOrDefaultAsync(c => c.Name == name);
    }
    public City? GetByNameAndCountry(string name, string country)
    {
        return _dbContext.Cities.FirstOrDefault(c => c.Name == name && c.Country == country);
    }

    public void Add(City city)
    {
        _dbContext.Add(city);
        _dbContext.SaveChanges();
    }

    public void Delete(City city)
    {
        _dbContext.Remove(city);
        _dbContext.SaveChanges();
    }

    public void Update(City city)
    {  
        _dbContext.Update(city);
        _dbContext.SaveChanges();
    }
}