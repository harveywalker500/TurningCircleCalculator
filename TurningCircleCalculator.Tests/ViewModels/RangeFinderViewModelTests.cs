using FluentAssertions;
using TurningCircleCalculator.Modules.RangeFinder;

namespace TurningCircleCalculator.Tests.ViewModels;

public class RangeFinderViewModelTests
{
    [Fact]
    public void Calculate_PdfExample_Returns1000m()
    {
        var vm = new RangeFinderViewModel
        {
            TrueHeightMeters = 30,
            ViewAngleCr = 3
        };
        vm.Calculate();
        vm.RangeMeters!.Value.Should().BeApproximately(1000, 1e-9);
        vm.RangeHm!.Value.Should().BeApproximately(10, 1e-9);
        vm.Error.Should().BeNull();
    }

    [Fact]
    public void Calculate_ZeroViewAngle_SurfacesError()
    {
        var vm = new RangeFinderViewModel
        {
            TrueHeightMeters = 30,
            ViewAngleCr = 0
        };
        vm.Calculate();
        vm.Error.Should().NotBeNullOrEmpty();
        vm.RangeMeters.Should().BeNull();
    }
}
