using FluentAssertions;
using TurningCircleCalculator.Models;
using TurningCircleCalculator.Modules.TurnPredictor;

namespace TurningCircleCalculator.Tests.ViewModels;

public class TurnPredictorViewModelTests
{
    [Fact]
    public void Calculate_AtTopSpeed_PopulatesRadius()
    {
        var vm = new TurnPredictorViewModel { SpeedKnots = 18 };
        vm.Calculate();
        vm.RadiusMeters.Should().NotBeNull();
        vm.RadiusMeters!.Value.Should().BeApproximately(330.43, 1e-1);
        vm.IsClamped.Should().BeFalse();
        vm.Error.Should().BeNull();
    }

    [Fact]
    public void Calculate_AtZeroSpeed_ClampsToFloor()
    {
        var vm = new TurnPredictorViewModel { SpeedKnots = 0 };
        vm.Calculate();
        vm.RadiusClampedMeters!.Value.Should().Be(NavConstants.MinTurnRadiusMeters);
        vm.IsClamped.Should().BeTrue();
    }

    [Fact]
    public void Calculate_NegativeSpeed_SurfacesError()
    {
        var vm = new TurnPredictorViewModel { SpeedKnots = -1 };
        vm.Calculate();
        vm.Error.Should().NotBeNullOrEmpty();
        vm.RadiusMeters.Should().BeNull();
    }
}
