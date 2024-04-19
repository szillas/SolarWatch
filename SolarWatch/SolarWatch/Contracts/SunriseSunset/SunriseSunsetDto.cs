namespace SolarWatch.Contracts.SunriseSunset;

public class SunriseSunsetDto
{
    public int Id { get; set; }
    public DateTime Date { get; set; }
    public string CityName { get; set; }
    public string Country { get; set; }
    public string Sunrise { get; set; }
    public string Sunset { get; set; }
    public string? TimeZone { get; set; }
}