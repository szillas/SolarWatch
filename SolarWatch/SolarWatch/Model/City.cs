namespace SolarWatch.Model;

public class City
{
    public int Id { get; init; }
    public string Name { get; init; }
    public double Latitude { get; init; }
    public double Longitude { get; init; }
    public string? State { get; init; }
    public string Country { get; init; }
}