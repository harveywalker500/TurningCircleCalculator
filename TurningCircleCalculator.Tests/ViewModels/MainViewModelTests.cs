using FluentAssertions;
using TurningCircleCalculator.Tests.Clock;
using TurningCircleCalculator.ViewModels;

namespace TurningCircleCalculator.Tests.ViewModels;

public class MainViewModelTests
{
    private static FakeClock MakeClock(int hour = 12, int minute = 0, int second = 0)
        => new(new DateTime(2024, 1, 1, hour, minute, second));

    [Fact]
    public void CurrentTime_ReflectsClockNow()
    {
        var clock = MakeClock(14, 30, 0);
        var vm = new MainViewModel(clock);
        vm.CurrentTime.Hour.Should().Be(14);
        vm.CurrentTime.Minute.Should().Be(30);
    }

    [Fact]
    public void IsClockPaused_ReflectsClockState()
    {
        var clock = MakeClock();
        var vm = new MainViewModel(clock);
        vm.IsClockPaused.Should().BeFalse();

        vm.PauseClock();
        vm.IsClockPaused.Should().BeTrue();

        vm.ResumeClock();
        vm.IsClockPaused.Should().BeFalse();
    }

    [Fact]
    public void SetClockTime_UpdatesCurrentTime()
    {
        var clock = MakeClock();
        var vm = new MainViewModel(clock);
        vm.SetClockTime("08:00:00");
        vm.CurrentTime.Hour.Should().Be(8);
    }

    [Fact]
    public void TrySetToi_ValidTime_ReturnsTrueAndSetsToiDisplayTime()
    {
        var clock = MakeClock(10, 0, 0);
        var vm = new MainViewModel(clock);
        var result = vm.TrySetToi("11:00:00");
        result.Should().BeTrue();
        vm.ToiDisplayTime.Should().Contain("11:00:00");
    }

    [Fact]
    public void TrySetToi_InvalidTime_ReturnsFalse()
    {
        var clock = MakeClock();
        var vm = new MainViewModel(clock);
        vm.TrySetToi("not-a-time").Should().BeFalse();
    }

    [Fact]
    public void Tick_BeforeImpact_ShowsCountdown()
    {
        var clock = MakeClock(10, 0, 0);
        var vm = new MainViewModel(clock);
        vm.TrySetToi("10:00:30");

        vm.Tick();

        vm.ToiCountdown.Should().StartWith("TOI:");
        vm.IsToiActive.Should().BeFalse();
    }

    [Fact]
    public void Tick_AtImpact_ShowsImpactAndFiresEvent()
    {
        var clock = MakeClock(10, 0, 0);
        var vm = new MainViewModel(clock);
        vm.TrySetToi("10:00:30"); // 30 s in the future

        clock.AdvanceSeconds(60); // advance past TOI

        var impactFired = false;
        vm.OnImpact += () => impactFired = true;
        vm.Tick();

        vm.ToiCountdown.Should().Be("IMPACT!");
        vm.IsToiActive.Should().BeTrue();
        impactFired.Should().BeTrue();
    }

    [Fact]
    public void OnImpact_FiresOnlyOnce()
    {
        var clock = MakeClock(10, 0, 0);
        var vm = new MainViewModel(clock);
        vm.TrySetToi("10:00:30");

        clock.AdvanceSeconds(60); // advance past TOI

        var count = 0;
        vm.OnImpact += () => count++;

        vm.Tick(); // should fire once
        vm.Tick(); // already active — must not fire again

        count.Should().Be(1);
    }

    [Fact]
    public void ToiCountdown_NoToi_ReturnsNotSet()
    {
        var clock = MakeClock();
        var vm = new MainViewModel(clock);
        vm.Tick();
        vm.ToiCountdown.Should().Be("TOI: Not Set");
    }
}
