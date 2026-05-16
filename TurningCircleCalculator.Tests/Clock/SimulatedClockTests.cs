using FluentAssertions;
using TurningCircleCalculator.Clock;

namespace TurningCircleCalculator.Tests.Clock;

public class SimulatedClockTests
{
    [Fact]
    public void ParseOrNow_ValidTime_ParsesCorrectly()
    {
        var result = SimulatedClock.ParseOrNow("14:30:00");
        result.Hour.Should().Be(14);
        result.Minute.Should().Be(30);
        result.Second.Should().Be(0);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("  ")]
    [InlineData("not-a-time")]
    public void ParseOrNow_InvalidOrEmpty_ReturnsCloseToNow(string? input)
    {
        var before = DateTime.Now;
        var result = SimulatedClock.ParseOrNow(input);
        result.Should().BeCloseTo(before, TimeSpan.FromSeconds(2));
    }

    [Fact]
    public void SetTime_UpdatesNow()
    {
        using var clock = new SimulatedClock("10:00:00");
        clock.SetTime("15:30:00");
        clock.Now.Hour.Should().Be(15);
        clock.Now.Minute.Should().Be(30);
    }

    [Fact]
    public void Pause_StopsAdvancing()
    {
        using var clock = new SimulatedClock("10:00:00");
        clock.Pause();
        clock.IsPaused.Should().BeTrue();
    }

    [Fact]
    public void Resume_RestartsAfterPause()
    {
        using var clock = new SimulatedClock("10:00:00");
        clock.Pause();
        clock.Resume();
        clock.IsPaused.Should().BeFalse();
    }

    [Fact]
    public void Dispose_CanBeCalledTwice()
    {
        var clock = new SimulatedClock("10:00:00");
        clock.Dispose();
        var act = () => clock.Dispose();
        act.Should().NotThrow();
    }

    [Fact]
    public void AdvancesOverTime()
    {
        using var clock = new SimulatedClock("10:00:00");
        var initial = clock.Now;
        Thread.Sleep(1100);
        clock.Now.Should().BeAfter(initial);
    }
}
