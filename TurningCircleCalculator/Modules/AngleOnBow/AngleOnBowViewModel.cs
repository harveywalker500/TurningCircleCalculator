using TurningCircleCalculator.Models;
using TurningCircleCalculator.ViewModels;

namespace TurningCircleCalculator.Modules.AngleOnBow;

/// <summary>
/// Backing ViewModel for the Angle on Bow module.
/// </summary>
public class AngleOnBowViewModel : ViewModelBase
{
    private double _rangeMeters;
    private double _viewAngleCr;
    private double _trueLengthMeters;
    private double? _aobDeg;
    private string? _error;

    public double RangeMeters
    {
        get => _rangeMeters;
        set { SetField(ref _rangeMeters, value); AobDeg = null; }
    }

    public double ViewAngleCr
    {
        get => _viewAngleCr;
        set { SetField(ref _viewAngleCr, value); AobDeg = null; }
    }

    public double TrueLengthMeters
    {
        get => _trueLengthMeters;
        set { SetField(ref _trueLengthMeters, value); AobDeg = null; }
    }

    public double? AobDeg
    {
        get => _aobDeg;
        private set => SetField(ref _aobDeg, value);
    }

    public string? Error
    {
        get => _error;
        private set => SetField(ref _error, value);
    }

    public void Calculate()
    {
        try
        {
            AobDeg = Optics.AngleOnBowFromLength(RangeMeters, ViewAngleCr, TrueLengthMeters);
            Error = null;
        }
        catch (ArgumentOutOfRangeException ex)
        {
            AobDeg = null;
            Error = ex.Message;
        }
    }
}
