namespace TurningCircleCalculator.Models;

/// <summary>
/// Nautical and U-Boat reference constants used throughout the calculator.
/// Values are taken from the WolfPack Krigsmarine Navigation Guide.
/// </summary>
public static class NavConstants
{
    /// <summary>One nautical mile expressed in meters.</summary>
    public const double NauticalMileMeters = 1852.0;

    /// <summary>Meters per second equivalent of one knot.</summary>
    public const double KnotMetersPerSecond = 0.51444;

    /// <summary>Meters per minute equivalent of one knot.</summary>
    public const double KnotMetersPerMinute = 30.8664;

    /// <summary>Side length in meters of a three-letter quadrant (e.g. AN 494).</summary>
    public const double LargeQuadrantMeters = 33336.0;

    /// <summary>Side length in meters of a four-number quadrant (e.g. AN 4943).</summary>
    public const double SmallQuadrantMeters = 11112.0;

    /// <summary>Minimum achievable turning radius for the Type VIIC U-Boat at full rudder.</summary>
    public const double MinTurnRadiusMeters = 102.0;

    /// <summary>Speed near which the minimum turning radius is reached.</summary>
    public const double MinTurnRadiusSpeedKnots = 5.0;

    /// <summary>Top surface speed of the Type VIIC U-Boat.</summary>
    public const double TopSpeedKnots = 18.0;

    /// <summary>Type VIIC hull length in meters.</summary>
    public const double TypeVIIC_LengthM = 67.2;

    /// <summary>Type VIIC beam in meters.</summary>
    public const double TypeVIIC_BeamM = 6.2;

    /// <summary>Type VIIC draught in meters.</summary>
    public const double TypeVIIC_DraughtM = 4.8;
}
