using System;

namespace TurningCircleCalculator.Models;

/// <summary>
/// Dead-reckoning distance calculations described in Chapter 5 of the
/// Krigsmarine Navigation Guide.
/// </summary>
public static class DeadReckoning
{
    /// <summary>
    /// Computes the distance travelled in meters at a given speed over an
    /// elapsed period.
    /// </summary>
    /// <param name="speedKnots">Speed in knots. Must be non-negative.</param>
    /// <param name="elapsed">Elapsed time. Must be non-negative.</param>
    /// <returns>Distance travelled in meters.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown for negative speed or elapsed time.</exception>
    public static double DistanceMeters(double speedKnots, TimeSpan elapsed)
    {
        if (speedKnots < 0)
            throw new ArgumentOutOfRangeException(nameof(speedKnots), "Speed must be non-negative.");
        if (elapsed < TimeSpan.Zero)
            throw new ArgumentOutOfRangeException(nameof(elapsed), "Elapsed time must be non-negative.");
        return speedKnots * NavConstants.KnotMetersPerMinute * elapsed.TotalMinutes;
    }

    /// <summary>
    /// Computes the distance travelled in nautical miles.
    /// </summary>
    /// <param name="speedKnots">Speed in knots. Must be non-negative.</param>
    /// <param name="elapsed">Elapsed time. Must be non-negative.</param>
    /// <returns>Distance travelled in nautical miles.</returns>
    public static double DistanceNauticalMiles(double speedKnots, TimeSpan elapsed)
        => DistanceMeters(speedKnots, elapsed) / NavConstants.NauticalMileMeters;
}
