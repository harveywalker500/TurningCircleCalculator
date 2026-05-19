using FluentAssertions;
using TurningCircleCalculator.Models;

namespace TurningCircleCalculator.Tests.Models;

public class OpticsTests
{
    [Fact]
    public void RangeMetersFromCentiradian_PdfExample_Returns1000()
    {
        // PDF pg 42: 30m / 3cr = 10hm = 1000m.
        Optics.RangeMetersFromCentiradian(30, 3).Should().BeApproximately(1000, 1e-9);
    }

    [Fact]
    public void RangeMetersFromCentiradian_ZeroViewAngle_Throws()
    {
        var act = () => Optics.RangeMetersFromCentiradian(30, 0);
        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void RangeMetersFromCentiradian_NegativeHeight_Throws()
    {
        var act = () => Optics.RangeMetersFromCentiradian(-1, 3);
        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void TargetSpeedKnots_PdfExample_Approx13_3()
    {
        // PDF pg 51: 2053m / 5min → ~13.30 knots
        Optics.TargetSpeedKnots(2053, 5).Should().BeApproximately(13.3024, 1e-3);
    }

    [Fact]
    public void TargetSpeedKnots_ZeroInterval_Throws()
    {
        var act = () => Optics.TargetSpeedKnots(2000, 0);
        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void AngleOnBowFromLength_PerpendicularCase_Returns90()
    {
        // Length 100m at range 100m, cr = 100 (so perceived length = full length).
        Optics.AngleOnBowFromLength(100, 100, 100).Should().BeApproximately(90, 1e-9);
    }

    [Fact]
    public void AngleOnBowFromLength_ClampsOversaturation()
    {
        // Deliberately oversaturate: ratio > 1 should clamp to asin(1) = 90°.
        Optics.AngleOnBowFromLength(1000, 50, 100).Should().BeApproximately(90, 1e-9);
    }

    [Fact]
    public void AngleOnBowFromLength_ZeroLengthThrows()
    {
        var act = () => Optics.AngleOnBowFromLength(1000, 5, 0);
        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void AngleOnBowFromLength_BowOn_Returns0()
    {
        // Zero view angle means the ship is end-on (perceived length 0).
        Optics.AngleOnBowFromLength(1000, 0, 100).Should().Be(0);
    }
}
