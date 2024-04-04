using SolarWatch.Data;
using SolarWatch.Model;

namespace SolarWatch.Services.Repository;

public class CityRepository : ICityRepository
{
    private SolarWatchApiContext _dbContext;

    public CityRepository(SolarWatchApiContext context)
    {
        _dbContext = context;
    }

    public IEnumerable<City> GetAll()
    {
        //var _dbContext = new SolarWatchApiContext();
        return _dbContext.Cities.ToList();
    }

    public City? GetByName(string name)
    {
        //var _dbContext = new SolarWatchApiContext();
        return _dbContext.Cities.FirstOrDefault(c => c.Name == name);
    }

    public void Add(City city)
    {
        //var _dbContext = new SolarWatchApiContext();
        _dbContext.Add(city);
        _dbContext.SaveChanges();
    }

    public void Delete(City city)
    {
        //var _dbContext = new SolarWatchApiContext();
        _dbContext.Remove(city);
        _dbContext.SaveChanges();
    }

    public void Update(City city)
    {  
        //var _dbContext = new SolarWatchApiContext();
        _dbContext.Update(city);
        _dbContext.SaveChanges();
    }
}