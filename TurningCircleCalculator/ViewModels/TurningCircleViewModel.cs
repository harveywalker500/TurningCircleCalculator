using TurningCircleCalculator.Models;

namespace TurningCircleCalculator.ViewModels;

public class TurningCircleViewModel : ViewModelBase
{
    private double _initialCourse;
    private double _newCourse;
    private double _odometer;
    private string _direction = "Left";
    private string _turnLog = string.Empty;
    private CalculationResult? _lastResult;
    private ChordExit? _lastChordExit;

    public double InitialCourse
    {
        get => _initialCourse;
        set => SetField(ref _initialCourse, value);
    }

    public double NewCourse
    {
        get => _newCourse;
        set => SetField(ref _newCourse, value);
    }

    public double Odometer
    {
        get => _odometer;
        set => SetField(ref _odometer, value);
    }

    public string Direction
    {
        get => _direction;
        set => SetField(ref _direction, value);
    }

    public string TurnLog
    {
        get => _turnLog;
        private set => SetField(ref _turnLog, value);
    }

    public CalculationResult? LastResult
    {
        get => _lastResult;
        private set => SetField(ref _lastResult, value);
    }

    public ChordExit? LastChordExit
    {
        get => _lastChordExit;
        private set => SetField(ref _lastChordExit, value);
    }

    public void Calculate(DateTime timestamp)
    {
        var label = Direction == "Left" ? "By the Left" : "By the Right";
        var result = Calculator.Calculate(InitialCourse, NewCourse, Odometer, label);
        LastResult = result;

        var turnDir = Direction == "Left" ? TurnDirection.Left : TurnDirection.Right;
        var chordExit = ChordGeometry.ComputeChordExit(InitialCourse, NewCourse, turnDir);
        LastChordExit = chordExit;

        var chordBearingText = chordExit.IsDegenerate ? "-" : $"{chordExit.ChordBearingDeg:F1}°";
        var entry = $"""
            [{timestamp:HH:mm:ss}] {result.Course1:F1}° -> {result.Course2:F1}° ({result.TurnDirection})
            Diff: {result.AngleDifference:F2}°, Rad: {(result.IsInfiniteRadius ? "Inf" : result.Radius.ToString("F2"))}, Chord: {(result.IsInfiniteRadius ? "-" : result.Chord.ToString("F2"))} @ {chordBearingText}
            ----------------------------------------
            """;

        TurnLog = entry + TurnLog;
        InitialCourse = result.Course2;
    }
}
