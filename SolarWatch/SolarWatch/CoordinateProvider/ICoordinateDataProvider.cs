namespace SolarWatch.CoordinateProvider;

public interface ICoordinateDataProvider
{
    Task<string> GetCoordinate(string city);
}