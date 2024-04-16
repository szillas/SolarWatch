using Microsoft.EntityFrameworkCore;
using SolarWatch.Model;

namespace SolarWatch.Data;

public class SolarWatchApiContext : DbContext
{
    public DbSet<City> Cities { get; set; }
    public DbSet<SunriseSunsetOfCity> SunriseSunsetOfCities { get; set; }

    public SolarWatchApiContext(DbContextOptions<SolarWatchApiContext> options) : base(options)
    {
    }
    
}