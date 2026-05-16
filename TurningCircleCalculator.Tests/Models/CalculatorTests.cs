using FluentAssertions;
using TurningCircleCalculator.Models;

namespace TurningCircleCalculator.Tests.Models;

public class CalculatorTests
{
    [Theory]
    [InlineData(0, 0)]
    [InlineData(360, 0)]
    [InlineData(720, 0)]
    [InlineData(-90, 270)]
    [InlineData(450, 90)]
    public void NormalizeBearing_ReturnsInRange(double input, double expected)
    {
        Calculator.NormalizeBearing(input).Should().BeApproximately(expected, 1e-9);
    }

    [Theory]
    [InlineData(0, 90, 90)]
    [InlineData(0, 180, 180)]
    [InlineData(0, 270, 90)]     // shortest path is 90°
    [InlineData(350, 10, 20)]    // crosses 0°
    public void SmallestAngleDifference_ReturnsShortestArc(double c1, double c2, double expected)
    {
        Calculator.SmallestAngleDifference(c1, c2).Should().BeApproximately(expected, 1e-9);
    }

    [Fact]
    public void Calculate_SameCourse_ReturnsInfiniteRadius()
    {
        var result = Calculator.Calculate(90, 90, 100, "By the Left");
        result.IsInfiniteRadius.Should().BeTrue();
        result.AngleDifference.Should().Be(0);
    }

    [Fact]
    public void Calculate_QuarterTurn_RadiusEqualsArcOverTheta()
    {
        // 90° turn, arc = 100 → radius = 100 / (π/2) ≈ 63.662
        var result = Calculator.Calculate(0, 90, 100, "By the Right");
        result.IsInfiniteRadius.Should().BeFalse();
        result.Radius.Should().BeApproximately(100.0 / (Math.PI / 2), 1e-6);
    }

    [Fact]
    public void Calculate_QuarterTurn_ChordIsDiagonalOfSquare()
    {
        // chord = 2r·sin(45°) = r·√2
        var result = Calculator.Calculate(0, 90, 100, "By the Right");
        var expectedChord = result.Radius * Math.Sqrt(2);
        result.Chord.Should().BeApproximately(expectedChord, 1e-6);
    }

    [Fact]
    public void Calculate_NormalizesInputBearings()
    {
        var result = Calculator.Calculate(370, 80, 100, "By the Left");
        result.Course1.Should().BeApproximately(10, 1e-9);
        result.Course2.Should().BeApproximately(80, 1e-9);
    }

    [Fact]
    public void Calculate_StoresTurnDirection()
    {
        var result = Calculator.Calculate(0, 90, 50, "By the Left");
        result.TurnDirection.Should().Be("By the Left");
    }
}
