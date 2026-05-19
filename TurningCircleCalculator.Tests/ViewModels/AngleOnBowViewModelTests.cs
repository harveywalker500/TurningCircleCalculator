using FluentAssertions;
using TurningCircleCalculator.Modules.AngleOnBow;

namespace TurningCircleCalculator.Tests.ViewModels;

public class AngleOnBowViewModelTests
{
    [Fact]
    public void Calculate_PerpendicularCase_Returns90()
    {
        var vm = new AngleOnBowViewModel
        {
            RangeMeters = 100,
            ViewAngleCr = 100,
            TrueLengthMeters = 100
        };
        vm.Calculate();
        vm.AobDeg!.Value.Should().BeApproximately(90, 1e-9);
        vm.Error.Should().BeNull();
    }

    [Fact]
    public void Calculate_ZeroLength_SurfacesError()
    {
        var vm = new AngleOnBowViewModel
        {
            RangeMeters = 1000,
            ViewAngleCr = 5,
            TrueLengthMeters = 0
        };
        vm.Calculate();
        vm.Error.Should().NotBeNullOrEmpty();
        vm.AobDeg.Should().BeNull();
    }
}
