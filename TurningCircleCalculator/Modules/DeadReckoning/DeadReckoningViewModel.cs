using TurningCircleCalculator.Models;
using TurningCircleCalculator.ViewModels;

namespace TurningCircleCalculator.Modules.DeadReckoning;

/// <summary>Operating mode for the Dead Reckoning module.</summary>
public enum DrMode
{
    /// <summary>Mark a start time and tick distance up from the live clock.</summary>
    Live,
    /// <summary>One-shot calculation from a user-typed elapsed interval.</summary>
    Manual
}

/// <summary>
/// Backing ViewModel for the Dead Reckoning module.
/// </summary>
public class DeadReckoningViewModel : ViewModelBase
{
    private double _speedKnots;
    private double _courseDeg;
    private DrMode _mode = DrMode.Live;
    private DateTime? _markTime;
    private TimeSpan _manualElapsed;
    private TimeSpan _liveElapsed;
    private double _distanceMeters;
    private double _distanceNm;
    private string? _error;

    public double SpeedKnots
    {
        get => _speedKnots;
        set { SetField(ref _speedKnots, value); Recompute(_markTime is null ? DateTime.MinValue : _markTime.Value); }
    }

    public double CourseDeg
    {
        get => _courseDeg;
        set => SetField(ref _courseDeg, value);
    }

    public DrMode Mode
    {
        get => _mode;
        set { SetField(ref _mode, value); ResetDistance(); }
    }

    public DateTime? MarkTime
    {
        get => _markTime;
        private set => SetField(ref _markTime, value);
    }

    public TimeSpan ManualElapsed
    {
        get => _manualElapsed;
        set { SetField(ref _manualElapsed, value); if (Mode == DrMode.Manual) ComputeFromElapsed(value); }
    }

    public TimeSpan LiveElapsed
    {
        get => _liveElapsed;
        private set => SetField(ref _liveElapsed, value);
    }

    public double DistanceMeters
    {
        get => _distanceMeters;
        private set => SetField(ref _distanceMeters, value);
    }

    public double DistanceNm
    {
        get => _distanceNm;
        private set => SetField(ref _distanceNm, value);
    }

    public string? Error
    {
        get => _error;
        private set => SetField(ref _error, value);
    }

    /// <summary>Mark the start of the dead-reckoning leg at <paramref name="now"/>.</summary>
    public void Mark(DateTime now)
    {
        MarkTime = now;
        LiveElapsed = TimeSpan.Zero;
        ResetDistance();
        Error = null;
    }

    /// <summary>Clear the live mark.</summary>
    public void ResetMark()
    {
        MarkTime = null;
        LiveElapsed = TimeSpan.Zero;
        ResetDistance();
    }

    /// <summary>Called by the host view on every poll-loop tick when in Live mode.</summary>
    public void Tick(DateTime now)
    {
        if (Mode != DrMode.Live) return;
        if (MarkTime is null) return;
        Recompute(now);
    }

    private void Recompute(DateTime now)
    {
        if (Mode != DrMode.Live || MarkTime is null) return;
        var elapsed = now - MarkTime.Value;
        if (elapsed < TimeSpan.Zero) elapsed = TimeSpan.Zero;
        LiveElapsed = elapsed;
        try
        {
            DistanceMeters = Models.DeadReckoning.DistanceMeters(SpeedKnots, elapsed);
            DistanceNm = Models.DeadReckoning.DistanceNauticalMiles(SpeedKnots, elapsed);
            Error = null;
        }
        catch (ArgumentOutOfRangeException ex)
        {
            ResetDistance();
            Error = ex.Message;
        }
    }

    private void ComputeFromElapsed(TimeSpan elapsed)
    {
        try
        {
            DistanceMeters = Models.DeadReckoning.DistanceMeters(SpeedKnots, elapsed);
            DistanceNm = Models.DeadReckoning.DistanceNauticalMiles(SpeedKnots, elapsed);
            Error = null;
        }
        catch (ArgumentOutOfRangeException ex)
        {
            ResetDistance();
            Error = ex.Message;
        }
    }

    private void ResetDistance()
    {
        DistanceMeters = 0;
        DistanceNm = 0;
    }
}
