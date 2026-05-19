using System;

namespace TurningCircleCalculator.Models;

/// <summary>
/// Optical range and angle calculations as described in the Krigsmarine
/// Navigation Guide, Chapter 7.
/// </summary>
public static class Optics
{
    /// <summary>
    /// Computes the range to a target from its known true height and the
    /// vertical view angle measured in centiradians (Guide pg 42).
    /// </summary>
    /// <param name="trueHeightMeters">The target's known height (e.g. mast height) in meters.</param>
    /// <param name="viewAngleCentiradians">The angular size of the height in centiradians (1 cr = 1 m at 100 m).</param>
    /// <returns>Range to the target in meters.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="viewAngleCentiradians"/> is zero or negative.</exception>
    public static double RangeMetersFromCentiradian(double trueHeightMeters, double viewAngleCentiradians)
    {
        if (viewAngleCentiradians <= 0)
            throw new ArgumentOutOfRangeException(nameof(viewAngleCentiradians), "View angle must be positive.");
        if (trueHeightMeters < 0)
            throw new ArgumentOutOfRangeException(nameof(trueHeightMeters), "Height must be non-negative.");
        // Range_hm = height / cr; Range_m = Range_hm * 100.
        return (trueHeightMeters / viewAngleCentiradians) * 100.0;
    }

    /// <summary>
    /// Computes the target's speed in knots from a measured distance over a
    /// time interval (Guide pg 51).
    /// </summary>
    /// <param name="distanceMeters">Distance the target travelled, in meters.</param>
    /// <param name="intervalMinutes">Elapsed time, in minutes.</param>
    /// <returns>Speed in knots.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="intervalMinutes"/> is zero or negative.</exception>
    public static double TargetSpeedKnots(double distanceMeters, double intervalMinutes)
    {
        if (intervalMinutes <= 0)
            throw new ArgumentOutOfRangeException(nameof(intervalMinutes), "Interval must be positive.");
        return (distanceMeters / intervalMinutes) / NavConstants.KnotMetersPerMinute;
    }

    /// <summary>
    /// Computes Angle on Bow from the optical view angle of the target's
    /// length and the known range and true length (Guide pg 56).
    /// </summary>
    /// <param name="rangeMeters">Range to the target in meters.</param>
    /// <param name="viewAngleCentiradians">Angular width of the target in centiradians.</param>
    /// <param name="trueLengthMeters">The target's true length in meters.</param>
    /// <returns>Angle on Bow in degrees, in <c>[0, 90]</c>.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when length is zero or negative.</exception>
    public static double AngleOnBowFromLength(double rangeMeters, double viewAngleCentiradians, double trueLengthMeters)
    {
        if (trueLengthMeters <= 0)
            throw new ArgumentOutOfRangeException(nameof(trueLengthMeters), "Length must be positive.");
        if (rangeMeters < 0)
            throw new ArgumentOutOfRangeException(nameof(rangeMeters), "Range must be non-negative.");
        if (viewAngleCentiradians < 0)
            throw new ArgumentOutOfRangeException(nameof(viewAngleCentiradians), "View angle must be non-negative.");

        // PDF formula: AoB = asin( (Range * cr) / Length ); clamp to absorb measurement noise.
        var ratio = (rangeMeters * viewAngleCentiradians) / (trueLengthMeters * 100.0);
        if (ratio > 1.0) ratio = 1.0;
        if (ratio < -1.0) ratio = -1.0;
        return Math.Asin(ratio) * 180.0 / Math.PI;
    }
}
