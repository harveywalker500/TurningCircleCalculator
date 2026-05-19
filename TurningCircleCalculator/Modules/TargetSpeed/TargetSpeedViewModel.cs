using TurningCircleCalculator.Models;
using TurningCircleCalculator.ViewModels;

namespace TurningCircleCalculator.Modules.TargetSpeed;

/// <summary>
/// Backing ViewModel for the Target Speed module.
/// </summary>
public class TargetSpeedViewModel : ViewModelBase
{
    private double _distanceMeters;
    private double _intervalMinutes;
    private double? _speedKnots;
    private string? _error;

    public double DistanceMeters
    {
        get => _distanceMeters;
        set { SetField(ref _distanceMeters, value); SpeedKnots = null; }
    }

    public double IntervalMinutes
    {
        get => _intervalMinutes;
        set { SetField(ref _intervalMinutes, value); SpeedKnots = null; }
    }

    public double? SpeedKnots
    {
        get => _speedKnots;
        private set
        {
            SetField(ref _speedKnots, value);
            OnPropertyChanged(nameof(SpeedMetersPerSecond));
            OnPropertyChanged(nameof(SpeedMetersPerMinute));
        }
    }

    public double? SpeedMetersPerSecond => SpeedKnots * NavConstants.KnotMetersPerSecond;
    public double? SpeedMetersPerMinute => SpeedKnots * NavConstants.KnotMetersPerMinute;

    public string? Error
    {
        get => _error;
        private set => SetField(ref _error, value);
    }

    public void Calculate()
    {
        try
        {
            SpeedKnots = Optics.TargetSpeedKnots(DistanceMeters, IntervalMinutes);
            Error = null;
        }
        catch (ArgumentOutOfRangeException ex)
        {
            SpeedKnots = null;
            Error = ex.Message;
        }
    }
}
