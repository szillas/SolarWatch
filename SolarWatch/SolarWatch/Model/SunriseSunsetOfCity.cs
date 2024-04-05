namespace SolarWatch.Model;

public class SunriseSunsetOfCity
{
    public int Id { get; init; }
    public DateTime Date { get; init; }
    public City City { get; init; }
    public string Sunrise { get; init; }
    public string Sunset { get; init; }
    public string? TimeZone { get; init; }
    
}