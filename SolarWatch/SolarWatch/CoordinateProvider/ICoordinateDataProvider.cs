namespace SolarWatch.CoordinateProvider;

public interface ICoordinateDataProvider
{
    string GetCoordinate(string city);
}