using FluentAssertions;
using TurningCircleCalculator.ViewModels;

namespace TurningCircleCalculator.Tests.ViewModels;

public class TurningCircleViewModelTests
{
    private static readonly DateTime Stamp = new(2024, 1, 1, 10, 0, 0);

    [Fact]
    public void Calculate_PopulatesLastResult()
    {
        var vm = new TurningCircleViewModel { InitialCourse = 0, NewCourse = 90, Odometer = 100, Direction = "Left" };

        vm.Calculate(Stamp);

        vm.LastResult.Should().NotBeNull();
        vm.LastResult!.IsInfiniteRadius.Should().BeFalse();
    }

    [Fact]
    public void Calculate_AdvancesInitialCourseToNewCourse()
    {
        var vm = new TurningCircleViewModel { InitialCourse = 0, NewCourse = 90, Odometer = 100 };
        vm.Calculate(Stamp);
        vm.InitialCourse.Should().BeApproximately(90, 1e-9);
    }

    [Fact]
    public void Calculate_SameCourse_IsInfiniteRadius()
    {
        var vm = new TurningCircleViewModel { InitialCourse = 45, NewCourse = 45, Odometer = 100 };
        vm.Calculate(Stamp);
        vm.LastResult!.IsInfiniteRadius.Should().BeTrue();
    }

    [Fact]
    public void Calculate_AppendsToTurnLog()
    {
        var vm = new TurningCircleViewModel { InitialCourse = 0, NewCourse = 90, Odometer = 100 };
        vm.Calculate(Stamp);
        vm.TurnLog.Should().Contain("[10:00:00]");
    }

    [Fact]
    public void Calculate_ChainedTurns_PrependsMostRecent()
    {
        var vm = new TurningCircleViewModel { InitialCourse = 0, NewCourse = 90, Odometer = 100 };
        vm.Calculate(new DateTime(2024, 1, 1, 10, 0, 0));
        vm.Calculate(new DateTime(2024, 1, 1, 10, 1, 0));
        vm.TurnLog.Should().StartWith("[10:01:00]");
    }

    [Fact]
    public void Direction_DefaultsToLeft()
    {
        new TurningCircleViewModel().Direction.Should().Be("Left");
    }

    [Fact]
    public void Calculate_RaisesPropertyChangedForLastResultAndTurnLog()
    {
        var vm = new TurningCircleViewModel { InitialCourse = 0, NewCourse = 90, Odometer = 100 };
        var raised = new List<string?>();
        vm.PropertyChanged += (_, e) => raised.Add(e.PropertyName);

        vm.Calculate(Stamp);

        raised.Should().Contain(nameof(TurningCircleViewModel.LastResult));
        raised.Should().Contain(nameof(TurningCircleViewModel.TurnLog));
    }
}
