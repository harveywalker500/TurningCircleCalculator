using FluentAssertions;
using TurningCircleCalculator.Models;

namespace TurningCircleCalculator.Tests.Models;

public class TurnPredictorTests
{
    [Fact]
    public void PredictRadiusMeters_AtZeroSpeed_ReturnsIntercept()
    {
        TurnPredictor.PredictRadiusMeters(0).Should().BeApproximately(2.93655892, 1e-6);
    }

    [Fact]
    public void PredictRadiusMeters_AtTopSpeed18kn_MatchesPdfExample()
    {
        // PDF pg 27 worked example: y ≈ 330.43
        TurnPredictor.PredictRadiusMeters(18).Should().BeApproximately(330.427889, 1e-4);
    }

    [Theory]
    [InlineData(5)]
    [InlineData(10)]
    [InlineData(15)]
    public void PredictRadiusMeters_PositiveSpeed_ReturnsPositive(double speed)
    {
        TurnPredictor.PredictRadiusMeters(speed).Should().BeGreaterThan(0);
    }

    [Fact]
    public void PredictRadiusMeters_NegativeSpeed_Throws()
    {
        var act = () => TurnPredictor.PredictRadiusMeters(-1);
        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void PredictRadiusMetersClamped_BelowFloor_ReturnsFloor()
    {
        // At s=0 raw ≈ 2.94 which is far below the 102m floor.
        TurnPredictor.PredictRadiusMetersClamped(0)
            .Should().Be(NavConstants.MinTurnRadiusMeters);
    }

    [Fact]
    public void PredictRadiusMetersClamped_AboveFloor_ReturnsRaw()
    {
        var raw = TurnPredictor.PredictRadiusMeters(18);
        TurnPredictor.PredictRadiusMetersClamped(18).Should().BeApproximately(raw, 1e-9);
    }
}
