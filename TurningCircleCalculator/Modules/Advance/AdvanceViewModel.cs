using TurningCircleCalculator.ViewModels;

namespace TurningCircleCalculator.Modules.Advance;

public class AdvanceViewModel : ViewModelBase
{
    private double _radius;
    private double _courseChange;
    private double? _advance;
    private double? _transfer;
    private string? _error;

    public double Radius
    {
        get => _radius;
        set { SetField(ref _radius, value); _advance = null; _transfer = null; _error = null; }
    }

    public double CourseChange
    {
        get => _courseChange;
        set { SetField(ref _courseChange, value); _advance = null; _transfer = null; _error = null; }
    }

    public double? Advance
    {
        get => _advance;
        private set => SetField(ref _advance, value);
    }

    public double? Transfer
    {
        get => _transfer;
        private set => SetField(ref _transfer, value);
    }

    public string? Error
    {
        get => _error;
        private set => SetField(ref _error, value);
    }

    public void Calculate()
    {
        if (Radius <= 0)
        {
            Error = "Radius must be > 0";
            return;
        }
        var theta = CourseChange * Math.PI / 180.0;
        Advance = Radius * Math.Sin(theta);
        Transfer = Radius * (1.0 - Math.Cos(theta));
        Error = null;
        OnPropertyChanged(nameof(Advance));
        OnPropertyChanged(nameof(Transfer));
    }
}
