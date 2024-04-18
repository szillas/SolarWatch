namespace SolarWatch.Model;

public class SunriseSunsetOfCity
{
    public int Id { get; init; }
    public DateTime Date { get; set; }
    public City City { get; set; }
    public string Sunrise { get; set; }
    public string Sunset { get; set; }
    public string? TimeZone { get; set; }
    
}