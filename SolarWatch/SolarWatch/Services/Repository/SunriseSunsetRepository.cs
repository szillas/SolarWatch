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
}