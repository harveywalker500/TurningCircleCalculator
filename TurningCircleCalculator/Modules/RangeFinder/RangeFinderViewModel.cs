using TurningCircleCalculator.Models;
using TurningCircleCalculator.ViewModels;

namespace TurningCircleCalculator.Modules.RangeFinder;

/// <summary>
/// Backing ViewModel for the optical range-finder module.
/// </summary>
public class RangeFinderViewModel : ViewModelBase
{
    private double _trueHeightMeters;
    private double _viewAngleCr;
    private double? _rangeMeters;
    private string? _error;

    public double TrueHeightMeters
    {
        get => _trueHeightMeters;
        set { SetField(ref _trueHeightMeters, value); RangeMeters = null; }
    }

    public double ViewAngleCr
    {
        get => _viewAngleCr;
        set { SetField(ref _viewAngleCr, value); RangeMeters = null; }
    }

    public double? RangeMeters
    {
        get => _rangeMeters;
        private set => SetField(ref _rangeMeters, value);
    }

    public double? RangeHm => RangeMeters / 100.0;

    public string? Error
    {
        get => _error;
        private set => SetField(ref _error, value);
    }

    public void Calculate()
    {
        try
        {
            RangeMeters = Optics.RangeMetersFromCentiradian(TrueHeightMeters, ViewAngleCr);
            Error = null;
            OnPropertyChanged(nameof(RangeHm));
        }
        catch (ArgumentOutOfRangeException ex)
        {
            RangeMeters = null;
            Error = ex.Message;
            OnPropertyChanged(nameof(RangeHm));
        }
    }
}
