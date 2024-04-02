namespace SolarWatch.Model;

public class Coordinate
{
    public double Longitude { get; set; }

    public double Latitude { get; set; }

    public override string ToString()
    {
        return $"{nameof(Longitude)}: {Longitude}, {nameof(Latitude)}: {Latitude}";
    }
}