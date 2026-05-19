using System;

namespace TurningCircleCalculator.Models;

/// <summary>The direction of a turn relative to the ship's bow.</summary>
public enum TurnDirection
{
    /// <summary>Port-side turn (anti-clockwise from above).</summary>
    Left,
    /// <summary>Starboard-side turn (clockwise from above).</summary>
    Right
}

/// <summary>
/// Result of a chord-exit calculation. All bearings are degrees from true
/// north, normalised to <c>[0, 360)</c>.
/// </summary>
/// <param name="TurnAngleDeg">The actual turn angle traversed, in degrees.</param>
/// <param name="EntryTangentBearingDeg">Bearing from the turning-circle centre to the entry point.</param>
/// <param name="ExitTangentBearingDeg">Bearing from the turning-circle centre to the exit point.</param>
/// <param name="ChordBearingDeg">True bearing of the chord drawn from entry to exit point.</param>
/// <param name="HalfAngleDeg">Base angle of the isosceles triangle formed by the chord and the two radii.</param>
/// <param name="IsDegenerate">True if the turn angle is zero (no turn).</param>
public readonly record struct ChordExit(
    double TurnAngleDeg,
    double EntryTangentBearingDeg,
    double ExitTangentBearingDeg,
    double ChordBearingDeg,
    double HalfAngleDeg,
    bool IsDegenerate);

/// <summary>
/// Computes the chord and tangent geometry described in the Krigsmarine
/// Navigation Guide, pp. 24-26.
/// </summary>
public static class ChordGeometry
{
    /// <summary>
    /// Computes the chord and tangent bearings for a turn from
    /// <paramref name="initialCourseDeg"/> to <paramref name="newCourseDeg"/>
    /// in the given <paramref name="direction"/>.
    /// </summary>
    /// <param name="initialCourseDeg">The course before the turn, in degrees.</param>
    /// <param name="newCourseDeg">The course after the turn, in degrees.</param>
    /// <param name="direction">The direction of the turn (port or starboard).</param>
    /// <returns>A <see cref="ChordExit"/> describing the turn's geometry.</returns>
    public static ChordExit ComputeChordExit(double initialCourseDeg, double newCourseDeg, TurnDirection direction)
    {
        var i = Calculator.NormalizeBearing(initialCourseDeg);
        var n = Calculator.NormalizeBearing(newCourseDeg);

        // Actual turn angle traversed, determined uniquely by direction.
        var delta = direction == TurnDirection.Right
            ? Calculator.NormalizeBearing(n - i)
            : Calculator.NormalizeBearing(i - n);

        if (delta == 0.0)
        {
            return new ChordExit(
                TurnAngleDeg: 0.0,
                EntryTangentBearingDeg: 0.0,
                ExitTangentBearingDeg: 0.0,
                ChordBearingDeg: i,
                HalfAngleDeg: 0.0,
                IsDegenerate: true);
        }

        double entryTangent, exitTangent, chordBearing;
        if (direction == TurnDirection.Right)
        {
            // Centre is on the starboard side; centre-to-entry points back at (i + 270°).
            entryTangent = Calculator.NormalizeBearing(i + 270.0);
            exitTangent = Calculator.NormalizeBearing(n + 270.0);
            chordBearing = Calculator.NormalizeBearing(i + (delta / 2.0));
        }
        else
        {
            // Centre is on the port side; centre-to-entry points back at (i + 90°).
            entryTangent = Calculator.NormalizeBearing(i + 90.0);
            exitTangent = Calculator.NormalizeBearing(n + 90.0);
            chordBearing = Calculator.NormalizeBearing(i - (delta / 2.0));
        }

        var halfAngle = (180.0 - delta) / 2.0;

        return new ChordExit(
            TurnAngleDeg: delta,
            EntryTangentBearingDeg: entryTangent,
            ExitTangentBearingDeg: exitTangent,
            ChordBearingDeg: chordBearing,
            HalfAngleDeg: halfAngle,
            IsDegenerate: false);
    }
}
