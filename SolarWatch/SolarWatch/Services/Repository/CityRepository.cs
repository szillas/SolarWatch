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
        _dbContext.Update(city);
        await _dbContext.SaveChangesAsync();
    }
}