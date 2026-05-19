using TurningCircleCalculator.Models;
using TurningCircleCalculator.ViewModels;

namespace TurningCircleCalculator.Modules.ChordExit;

/// <summary>
/// Backing ViewModel for the standalone Chord Exit Angle module.
/// </summary>
public class ChordExitViewModel : ViewModelBase
{
    private double _initialCourse;
    private double _newCourse;
    private TurnDirection _direction = TurnDirection.Right;
    private Models.ChordExit? _result;

    public double InitialCourse
    {
        get => _initialCourse;
        set { SetField(ref _initialCourse, value); Result = null; }
    }

    public double NewCourse
    {
        get => _newCourse;
        set { SetField(ref _newCourse, value); Result = null; }
    }

    public TurnDirection Direction
    {
        get => _direction;
        set { SetField(ref _direction, value); Result = null; }
    }

    public Models.ChordExit? Result
    {
        get => _result;
        private set => SetField(ref _result, value);
    }

    public void Calculate()
    {
        Result = ChordGeometry.ComputeChordExit(InitialCourse, NewCourse, Direction);
    }
}
