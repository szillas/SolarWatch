using SolarWatch.Model;

namespace SolarWatch.Services.Repository;

public interface ISunriseSunsetRepository
{
    Task<IEnumerable<SunriseSunsetOfCity>> GetAll();
    Task<SunriseSunsetOfCity?> GetByDateAndCity(string city, DateTime date);
    Task<SunriseSunsetOfCity?> GetByDateAndCity(string city, string? date);

    Task Add(SunriseSunsetOfCity sunriseSunset);
    Task Delete(SunriseSunsetOfCity sunriseSunset);
    Task Update(SunriseSunsetOfCity sunriseSunset);
}