using System;

namespace TurningCircleCalculator.Models;

/// <summary>
/// A static utility class for performing nautical course and distance calculations.
/// </summary>
public static class Calculator
{
    /// <summary>
    /// Normalizes a bearing value to be within the range [0, 360).
    /// </summary>
    /// <param name="bearing">The bearing value in degrees.</param>
    /// <returns>The normalized bearing in degrees.</returns>
    public static double NormalizeBearing(double bearing)
    {
        bearing %= 360.0;
        if (bearing < 0)
            bearing += 360.0;
        return bearing;
    }

    /// <summary>
    /// Calculates the smallest angle difference between two courses.
    /// </summary>
    /// <param name="c1">The first course in degrees.</param>
    /// <param name="c2">The second course in degrees.</param>
    /// <returns>The smallest difference in degrees, between 0 and 180.</returns>
    public static double SmallestAngleDifference(double c1, double c2)
    {
        double diff = Math.Abs(c2 - c1);
        return diff > 180.0 ? 360.0 - diff : diff;
    }

    /// <summary>
    /// Converts a value from degrees to radians.
    /// </summary>
    /// <param name="degrees">The value in degrees.</param>
    /// <returns>The value in radians.</returns>
    public static double DegreesToRadians(double degrees)
    {
        return degrees * Math.PI / 180.0;
    }

    /// <summary>
    /// Calculates the turn details (radius, chord, angle difference) based on course changes and odometer distance.
    /// </summary>
    /// <param name="currentCourse">The initial course in degrees.</param>
    /// <param name="newCourse">The final course in degrees.</param>
    /// <param name="odometer">The distance traveled during the turn (e.g., in nautical miles or meters).</param>
    /// <param name="direction">The direction of the turn (e.g., "Left", "Right").</param>
    /// <returns>A <see cref="CalculationResult"/> containing the calculated turn parameters.</returns>
    public static CalculationResult Calculate(double currentCourse, double newCourse, double odometer, string direction)
    {
        currentCourse = NormalizeBearing(currentCourse);
        newCourse = NormalizeBearing(newCourse);
        double deltaThetaDeg = SmallestAngleDifference(currentCourse, newCourse);
        double thetaRad = DegreesToRadians(deltaThetaDeg);

        if (thetaRad == 0)
        {
            return new CalculationResult
            {
                Course1 = currentCourse,
                Course2 = newCourse,
                AngleDifference = deltaThetaDeg,
                IsInfiniteRadius = true,
                TurnDirection = direction
            };
        }

        double radius = odometer / thetaRad;
        double chord = 2 * radius * Math.Sin(thetaRad / 2.0);

        return new CalculationResult
        {
            Course1 = currentCourse,
            Course2 = newCourse,
            AngleDifference = deltaThetaDeg,
            Radius = radius,
            Chord = chord,
            TurnDirection = direction,
            IsInfiniteRadius = false
        };
    }
}

/// <summary>
/// Represents the result of a turning circle calculation.
/// </summary>
public class CalculationResult
{
    /// <summary>
    /// Gets or sets the initial course.
    /// </summary>
    public double Course1 { get; set; }

    /// <summary>
    /// Gets or sets the final course.
    /// </summary>
    public double Course2 { get; set; }

    /// <summary>
    /// Gets or sets the angle difference between courses in degrees.
    /// </summary>
    public double AngleDifference { get; set; }

    /// <summary>
    /// Gets or sets the radius of the turning circle.
    /// </summary>
    public double Radius { get; set; }

    /// <summary>
    /// Gets or sets the chord distance between the start and end of the turn.
    /// </summary>
    public double Chord { get; set; }

    /// <summary>
    /// Gets or sets the direction of the turn.
    /// </summary>
    public string TurnDirection { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets a value indicating whether the turn has an infinite radius (i.e., straight line).
    /// </summary>
    public bool IsInfiniteRadius { get; set; }
}
