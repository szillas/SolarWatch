using SolarWatch.Model;

namespace SolarWatch.Services.Repository;

public interface ISunriseSunsetRepository
{
    IEnumerable<SunriseSunsetOfCity> GetAll();
    SunriseSunsetOfCity? GetByDateAndCity(int cityId, DateTime date);

    void Add(SunriseSunsetOfCity sunriseSunset);
    void Delete(SunriseSunsetOfCity sunriseSunset);
    void Update(SunriseSunsetOfCity sunriseSunset);
}