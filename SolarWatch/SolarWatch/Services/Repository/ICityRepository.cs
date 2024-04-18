using SolarWatch.Model;

namespace SolarWatch.Services.Repository;

public interface ICityRepository
{
    Task<IEnumerable<City>> GetAll();
    Task<City?> GetByName(string name);
    City? GetByNameAndCountry(string name, string country);
    void Add(City city);
    void Delete(City city);
    void Update(City city);
}