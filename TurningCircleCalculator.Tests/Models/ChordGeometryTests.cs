using FluentAssertions;
using TurningCircleCalculator.Models;

namespace TurningCircleCalculator.Tests.Models;

public class ChordGeometryTests
{
    [Fact]
    public void RightTurn_NorthToEast_ChordBearingIs45()
    {
        var ce = ChordGeometry.ComputeChordExit(0, 90, TurnDirection.Right);
        ce.IsDegenerate.Should().BeFalse();
        ce.TurnAngleDeg.Should().BeApproximately(90, 1e-9);
        ce.ChordBearingDeg.Should().BeApproximately(45, 1e-9);
        ce.HalfAngleDeg.Should().BeApproximately(45, 1e-9);
        ce.EntryTangentBearingDeg.Should().BeApproximately(270, 1e-9);
        ce.ExitTangentBearingDeg.Should().BeApproximately(0, 1e-9);
    }

    [Fact]
    public void LeftTurn_NorthToWest_ChordBearingIs315()
    {
        // 0° → 270° turning left: 90° of left turn.
        var ce = ChordGeometry.ComputeChordExit(0, 270, TurnDirection.Left);
        ce.IsDegenerate.Should().BeFalse();
        ce.TurnAngleDeg.Should().BeApproximately(90, 1e-9);
        ce.ChordBearingDeg.Should().BeApproximately(315, 1e-9);
        ce.HalfAngleDeg.Should().BeApproximately(45, 1e-9);
        ce.EntryTangentBearingDeg.Should().BeApproximately(90, 1e-9);
        ce.ExitTangentBearingDeg.Should().BeApproximately(0, 1e-9);
    }

    [Fact]
    public void RightTurn_WrapsAcrossNorth()
    {
        // 350° → 20° turning right is a 30° turn; chord bearing = 350 + 15 = 5°.
        var ce = ChordGeometry.ComputeChordExit(350, 20, TurnDirection.Right);
        ce.TurnAngleDeg.Should().BeApproximately(30, 1e-9);
        ce.ChordBearingDeg.Should().BeApproximately(5, 1e-9);
    }

    [Fact]
    public void LeftTurn_GoingTheLongWay_Has270DegreeTurnAngle()
    {
        // 0° → 90° turning left means going 270° anticlockwise.
        var ce = ChordGeometry.ComputeChordExit(0, 90, TurnDirection.Left);
        ce.TurnAngleDeg.Should().BeApproximately(270, 1e-9);
        // chord bearing = (0 - 135) mod 360 = 225°
        ce.ChordBearingDeg.Should().BeApproximately(225, 1e-9);
    }

    [Fact]
    public void HalfAngle_Is180MinusDeltaOverTwo()
    {
        var ce = ChordGeometry.ComputeChordExit(0, 60, TurnDirection.Right);
        ce.TurnAngleDeg.Should().BeApproximately(60, 1e-9);
        ce.HalfAngleDeg.Should().BeApproximately((180.0 - 60.0) / 2.0, 1e-9);
    }

    [Fact]
    public void NoTurn_ReturnsDegenerate()
    {
        var ce = ChordGeometry.ComputeChordExit(45, 45, TurnDirection.Right);
        ce.IsDegenerate.Should().BeTrue();
        ce.TurnAngleDeg.Should().Be(0);
        ce.ChordBearingDeg.Should().BeApproximately(45, 1e-9);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(45)]
    [InlineData(120)]
    [InlineData(200)]
    [InlineData(330)]
    public void TangentsAreNormalizedToZeroThreeSixty(double initialCourse)
    {
        var ce = ChordGeometry.ComputeChordExit(initialCourse, initialCourse + 73, TurnDirection.Right);
        ce.EntryTangentBearingDeg.Should().BeInRange(0, 360 - 1e-9);
        ce.ExitTangentBearingDeg.Should().BeInRange(0, 360 - 1e-9);
        ce.ChordBearingDeg.Should().BeInRange(0, 360 - 1e-9);
    }
}
