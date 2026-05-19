using TurningCircleCalculator.Models;
using TurningCircleCalculator.ViewModels;

namespace TurningCircleCalculator.Modules.TurnPredictor;

/// <summary>
/// Backing ViewModel for the Turn Radius Predictor module.
/// </summary>
public class TurnPredictorViewModel : ViewModelBase
{
    private double _speedKnots;
    private double? _radiusMeters;
    private double? _radiusClampedMeters;
    private bool _isClamped;
    private string? _error;

    public double SpeedKnots
    {
        get => _speedKnots;
        set { SetField(ref _speedKnots, value); ClearResults(); }
    }

    public double? RadiusMeters
    {
        get => _radiusMeters;
        private set => SetField(ref _radiusMeters, value);
    }

    public double? RadiusClampedMeters
    {
        get => _radiusClampedMeters;
        private set => SetField(ref _radiusClampedMeters, value);
    }

    public bool IsClamped
    {
        get => _isClamped;
        private set => SetField(ref _isClamped, value);
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
            var raw = Models.TurnPredictor.PredictRadiusMeters(SpeedKnots);
            var clamped = Models.TurnPredictor.PredictRadiusMetersClamped(SpeedKnots);
            RadiusMeters = raw;
            RadiusClampedMeters = clamped;
            IsClamped = clamped > raw;
            Error = null;
        }
        catch (ArgumentOutOfRangeException ex)
        {
            ClearResults();
            Error = ex.Message;
        }
    }

    private void ClearResults()
    {
        RadiusMeters = null;
        RadiusClampedMeters = null;
        IsClamped = false;
        Error = null;
    }
}
