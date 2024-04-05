using SolarWatch.Model;

namespace SolarWatch.Services.Repository;

public interface ISunriseSunsetRepository
{
    IEnumerable<SunriseSunsetOfCity> GetAll();
    SunriseSunsetOfCity? GetByDateAndCity(string city, DateTime date);
    SunriseSunsetOfCity? GetByDateAndCity(string city, string? date);

    void Add(SunriseSunsetOfCity sunriseSunset);
    void Delete(SunriseSunsetOfCity sunriseSunset);
    void Update(SunriseSunsetOfCity sunriseSunset);
}