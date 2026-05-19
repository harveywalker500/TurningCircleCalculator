using FluentAssertions;
using TurningCircleCalculator.Models;

namespace TurningCircleCalculator.Tests.Models;

public class DeadReckoningTests
{
    [Fact]
    public void DistanceMeters_FiveKnotsOverOneHour_IsFiveNauticalMiles()
    {
        // 5 kn × 60 min × 30.8664 m/min ≈ 9259.92 m ≈ 5 NM (5 × 1852 = 9260).
        var d = DeadReckoning.DistanceMeters(5, TimeSpan.FromHours(1));
        d.Should().BeApproximately(5 * 60 * NavConstants.KnotMetersPerMinute, 1e-6);
        d.Should().BeApproximately(9259.92, 0.1);
    }

    [Fact]
    public void DistanceMeters_ZeroElapsed_IsZero()
    {
        DeadReckoning.DistanceMeters(18, TimeSpan.Zero).Should().Be(0);
    }

    [Fact]
    public void DistanceMeters_NegativeSpeed_Throws()
    {
        var act = () => DeadReckoning.DistanceMeters(-1, TimeSpan.FromMinutes(1));
        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void DistanceMeters_NegativeElapsed_Throws()
    {
        var act = () => DeadReckoning.DistanceMeters(5, TimeSpan.FromMinutes(-1));
        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void DistanceNauticalMiles_RoundTrips()
    {
        var d_m = DeadReckoning.DistanceMeters(10, TimeSpan.FromMinutes(30));
        var d_nm = DeadReckoning.DistanceNauticalMiles(10, TimeSpan.FromMinutes(30));
        (d_nm * NavConstants.NauticalMileMeters).Should().BeApproximately(d_m, 1e-6);
    }
}
