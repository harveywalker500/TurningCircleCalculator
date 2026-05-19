using FluentAssertions;
using TurningCircleCalculator.Models;
using TurningCircleCalculator.Modules.ChordExit;

namespace TurningCircleCalculator.Tests.ViewModels;

public class ChordExitViewModelTests
{
    [Fact]
    public void Calculate_RightTurn_PopulatesResult()
    {
        var vm = new ChordExitViewModel
        {
            InitialCourse = 0,
            NewCourse = 90,
            Direction = TurnDirection.Right
        };
        vm.Calculate();
        vm.Result.Should().NotBeNull();
        vm.Result!.Value.ChordBearingDeg.Should().BeApproximately(45, 1e-9);
    }

    [Fact]
    public void SettingInput_ClearsPreviousResult()
    {
        var vm = new ChordExitViewModel
        {
            InitialCourse = 0,
            NewCourse = 90,
            Direction = TurnDirection.Right
        };
        vm.Calculate();
        vm.Result.Should().NotBeNull();

        vm.NewCourse = 180;
        vm.Result.Should().BeNull();
    }
}
