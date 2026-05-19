using FluentAssertions;
using TurningCircleCalculator.Modules.DeadReckoning;
using TurningCircleCalculator.Tests.Clock;

namespace TurningCircleCalculator.Tests.ViewModels;

public class DeadReckoningViewModelTests
{
    [Fact]
    public void LiveMode_MarkAndTick_AdvancesDistance()
    {
        var clock = new FakeClock(new DateTime(2024, 1, 1, 12, 0, 0));
        var vm = new DeadReckoningViewModel
        {
            Mode = DrMode.Live,
            SpeedKnots = 10
        };
        vm.Mark(clock.Now);

        clock.AdvanceSeconds(1800); // 30 minutes
        vm.Tick(clock.Now);

        // 10 kn × 30 min × 30.8664 m/min = 9259.92 m
        vm.DistanceMeters.Should().BeApproximately(9259.92, 0.1);
        vm.DistanceNm.Should().BeApproximately(9259.92 / 1852.0, 1e-3);
        vm.LiveElapsed.Should().Be(TimeSpan.FromMinutes(30));
        vm.Error.Should().BeNull();
    }

    [Fact]
    public void LiveMode_WithoutMark_TickDoesNothing()
    {
        var clock = new FakeClock();
        var vm = new DeadReckoningViewModel { Mode = DrMode.Live, SpeedKnots = 10 };
        clock.AdvanceSeconds(600);
        vm.Tick(clock.Now);
        vm.DistanceMeters.Should().Be(0);
    }

    [Fact]
    public void LiveMode_ResetMark_FreezesDistance()
    {
        var clock = new FakeClock();
        var vm = new DeadReckoningViewModel { Mode = DrMode.Live, SpeedKnots = 10 };
        vm.Mark(clock.Now);
        clock.AdvanceSeconds(600);
        vm.Tick(clock.Now);
        vm.DistanceMeters.Should().BeGreaterThan(0);

        vm.ResetMark();
        vm.DistanceMeters.Should().Be(0);
        vm.MarkTime.Should().BeNull();
    }

    [Fact]
    public void LiveMode_ClockDoesNotAdvance_DistanceUnchanged()
    {
        // Simulates SimulatedClock.Pause() — Clock.Now does not progress.
        var clock = new FakeClock();
        var vm = new DeadReckoningViewModel { Mode = DrMode.Live, SpeedKnots = 18 };
        vm.Mark(clock.Now);
        clock.AdvanceSeconds(60);
        vm.Tick(clock.Now);
        var before = vm.DistanceMeters;

        // No clock advance: subsequent ticks should leave distance unchanged.
        vm.Tick(clock.Now);
        vm.Tick(clock.Now);
        vm.DistanceMeters.Should().Be(before);
    }

    [Fact]
    public void ManualMode_SettingElapsed_ComputesDistance()
    {
        var vm = new DeadReckoningViewModel
        {
            Mode = DrMode.Manual,
            SpeedKnots = 5
        };
        vm.ManualElapsed = TimeSpan.FromHours(1);
        // 5 kn × 60 min × 30.8664 = 9259.92
        vm.DistanceMeters.Should().BeApproximately(9259.92, 0.1);
    }

    [Fact]
    public void ManualMode_LiveTickIsNoOp()
    {
        var clock = new FakeClock();
        var vm = new DeadReckoningViewModel
        {
            Mode = DrMode.Manual,
            SpeedKnots = 10
        };
        // Even though we mark and advance, Tick should ignore in manual mode.
        vm.Mark(clock.Now);
        clock.AdvanceSeconds(600);
        vm.Tick(clock.Now);
        vm.DistanceMeters.Should().Be(0);
    }
}
