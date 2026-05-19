using FluentAssertions;
using TurningCircleCalculator.Modules.TargetSpeed;

namespace TurningCircleCalculator.Tests.ViewModels;

public class TargetSpeedViewModelTests
{
    [Fact]
    public void Calculate_PdfExample_Returns13_3Knots()
    {
        var vm = new TargetSpeedViewModel
        {
            DistanceMeters = 2053,
            IntervalMinutes = 5
        };
        vm.Calculate();
        vm.SpeedKnots!.Value.Should().BeApproximately(13.3024, 1e-3);
        vm.SpeedMetersPerSecond!.Value.Should().BeApproximately(13.3024 * 0.51444, 1e-2);
        vm.SpeedMetersPerMinute!.Value.Should().BeApproximately(13.3024 * 30.8664, 1e-1);
        vm.Error.Should().BeNull();
    }

    [Fact]
    public void Calculate_ZeroInterval_SurfacesError()
    {
        var vm = new TargetSpeedViewModel
        {
            DistanceMeters = 2000,
            IntervalMinutes = 0
        };
        vm.Calculate();
        vm.Error.Should().NotBeNullOrEmpty();
        vm.SpeedKnots.Should().BeNull();
    }
}
