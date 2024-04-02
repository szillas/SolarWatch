using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SolarWatch;
using SolarWatch.Controllers;
using SolarWatch.CoordinateProvider;
using SolarWatch.JsonProcessor;
using SolarWatch.SunriseSunsetProvider;

namespace SolarWatchTest;

[TestFixture]
public class SunriseSunsetControllerTest
{
    private Mock<ILogger<SunriseSunsetController>> _loggerMock;
    private Mock<ICoordinateDataProvider> _coordinateDataProviderMock;
    private Mock<ISunriseSunsetProvider> _sunriseSunsetProviderMock;
    private Mock<IJsonProcessor> _jsonProcessorMock;
    private SunriseSunsetController _controller;

    [SetUp]
    public void SetUp()
    {
        _loggerMock = new Mock<ILogger<SunriseSunsetController>>();
        _coordinateDataProviderMock = new Mock<ICoordinateDataProvider>();
        _sunriseSunsetProviderMock = new Mock<ISunriseSunsetProvider>();
        _jsonProcessorMock = new Mock<IJsonProcessor>();
        _controller = new SunriseSunsetController(_loggerMock.Object, _coordinateDataProviderMock.Object,
            _jsonProcessorMock.Object, _sunriseSunsetProviderMock.Object);
    }
}