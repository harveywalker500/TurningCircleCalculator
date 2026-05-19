using System;

namespace TurningCircleCalculator.Models;

/// <summary>
/// Predicts the radius of the Type VIIC U-Boat's turning circle at a given
/// speed and full rudder deflection (35°), using the regression curve
/// derived in the Krigsmarine Navigation Guide (pg 27).
/// </summary>
public static class TurnPredictor
{
    private const double A = 0.026413725;
    private const double B = 17.71851559;
    private const double C = 2.93655892;

    /// <summary>
    /// Predicts the turning radius in meters for the supplied speed in knots.
    /// </summary>
    /// <param name="speedKnots">Speed of the U-Boat in knots. Must be non-negative.</param>
    /// <returns>Predicted radius in meters.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="speedKnots"/> is negative.</exception>
    public static double PredictRadiusMeters(double speedKnots)
    {
        if (speedKnots < 0)
            throw new ArgumentOutOfRangeException(nameof(speedKnots), "Speed must be non-negative.");
        return (A * speedKnots * speedKnots) + (B * speedKnots) + C;
    }

    /// <summary>
    /// Predicts the turning radius and clamps it to the U-Boat's physical
    /// minimum radius (<see cref="NavConstants.MinTurnRadiusMeters"/>).
    /// </summary>
    /// <param name="speedKnots">Speed of the U-Boat in knots. Must be non-negative.</param>
    /// <returns>Predicted radius in meters, never less than the minimum.</returns>
    public static double PredictRadiusMetersClamped(double speedKnots)
    {
        var raw = PredictRadiusMeters(speedKnots);
        return raw < NavConstants.MinTurnRadiusMeters ? NavConstants.MinTurnRadiusMeters : raw;
    }
}
