using System.ComponentModel;
using System.Runtime.CompilerServices;
using TurningCircleCalculator.Models;

namespace TurningCircleCalculator.ViewModels;

public class MainViewModel : INotifyPropertyChanged
{
    private DateTime _currentTime;
    private double _initialCourse;
    private double _newCourse;
    private double _odometer;
    private string _direction = "Left";
    private string _turnLog = string.Empty;
    private CalculationResult _lastResult;

    private DateTime? _toiTime;
    private bool _isToiActive;
    private string _toiCountdown = string.Empty;
    private string _toiDisplayTime = "TOI @ --:--:--";

    public event PropertyChangedEventHandler? PropertyChanged;

    public DateTime CurrentTime
    {
        get => _currentTime;
        set { _currentTime = value; OnPropertyChanged(); UpdateToiCountdown(); }
    }

    public double InitialCourse
    {
        get => _initialCourse;
        set { _initialCourse = value; OnPropertyChanged(); }
    }

    public double NewCourse
    {
        get => _newCourse;
        set { _newCourse = value; OnPropertyChanged(); }
    }

    public double Odometer
    {
        get => _odometer;
        set { _odometer = value; OnPropertyChanged(); }
    }

    public string Direction
    {
        get => _direction;
        set { _direction = value; OnPropertyChanged(); }
    }

    public string TurnLog
    {
        get => _turnLog;
        set { _turnLog = value; OnPropertyChanged(); }
    }

    public CalculationResult LastResult
    {
        get => _lastResult;
        set { _lastResult = value; OnPropertyChanged(); }
    }

    public DateTime? ToiTime
    {
        get => _toiTime;
        set { _toiTime = value; OnPropertyChanged(); UpdateToiCountdown(); }
    }

    public bool IsToiActive
    {
        get => _isToiActive;
        set { _isToiActive = value; OnPropertyChanged(); }
    }

    public string ToiCountdown
    {
        get => _toiCountdown;
        set { _toiCountdown = value; OnPropertyChanged(); }
    }

    public string ToiDisplayTime
    {
        get => _toiDisplayTime;
        set { _toiDisplayTime = value; OnPropertyChanged(); }
    }

    public event Action? OnImpact;

    public void Calculate()
    {
        var result = Models.Calculator.Calculate(InitialCourse, NewCourse, Odometer, Direction == "Left" ? "By the Left" : "By the Right");
        LastResult = result;

        var logEntry = $"""
            [{CurrentTime:HH:mm:ss}] {result.Course1:F1}° -> {result.Course2:F1}° ({result.TurnDirection})
            Diff: {result.AngleDifference:F2}°, Rad: {(result.IsInfiniteRadius ? "Inf" : result.Radius.ToString("F2"))}, Chord: {(result.IsInfiniteRadius ? "-" : result.Chord.ToString("F2"))}
            ----------------------------------------
            """;
        
        TurnLog = logEntry + TurnLog;
        InitialCourse = result.Course2;
    }

    private void UpdateToiCountdown()
    {
        if (ToiTime == null)
        {
            ToiCountdown = "TOI: Not Set";
            ToiDisplayTime = "TOI @ --:--:--";
            IsToiActive = false;
            return;
        }

        ToiDisplayTime = $"TOI @ {ToiTime.Value:HH:mm:ss}";

        var diff = ToiTime.Value - CurrentTime;
        if (diff.TotalSeconds <= 0)
        {
            ToiCountdown = "IMPACT!";
            if (!IsToiActive)
            {
                IsToiActive = true;
                OnImpact?.Invoke();
            }
        }
        else
        {
            ToiCountdown = $"TOI: {diff:hh\\:mm\\:ss}";
            IsToiActive = false;
        }
    }

    protected void OnPropertyChanged([CallerMemberName] string? name = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
