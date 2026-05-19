using Terminal.Gui;
using TurningCircleCalculator.Models;

namespace TurningCircleCalculator.Modules.ChordExit;

/// <summary>
/// View for the Chord Exit Angle module.
/// </summary>
public class ChordExitView : View
{
    private readonly ChordExitViewModel _vm = new();
    private readonly TextField _initialField;
    private readonly TextField _newField;
    private readonly RadioGroup _directionRadio;
    private readonly Label _chordValue;
    private readonly Label _entryValue;
    private readonly Label _exitValue;
    private readonly Label _halfAngleValue;
    private readonly Label _turnAngleValue;
    private readonly Label _diagram;

    private const int LabelWidth = 20;

    public ChordExitView()
    {
        var lblInit = new Label("Initial Course (°):") { X = 1, Y = 0 };
        _initialField = new TextField("") { X = LabelWidth, Y = 0, Width = 10 };
        _initialField.TextChanged += _ =>
        {
            if (double.TryParse(_initialField.Text.ToString(), out var v)) _vm.InitialCourse = v;
        };

        var lblNew = new Label("New Course (°):") { X = 1, Y = 2 };
        _newField = new TextField("") { X = LabelWidth, Y = 2, Width = 10 };
        _newField.TextChanged += _ =>
        {
            if (double.TryParse(_newField.Text.ToString(), out var v)) _vm.NewCourse = v;
        };

        var lblDir = new Label("Direction:") { X = 1, Y = 4 };
        _directionRadio = new RadioGroup(new NStack.ustring[] { "Left (Port)", "Right (Starboard)" })
        {
            X = LabelWidth, Y = 4, SelectedItem = 1
        };
        _directionRadio.SelectedItemChanged += e =>
            _vm.Direction = e.SelectedItem == 0 ? TurnDirection.Left : TurnDirection.Right;

        var calcBtn = new Button("Calculate") { X = 1, Y = 7 };
        calcBtn.Clicked += () => _vm.Calculate();

        var resultsFrame = new FrameView("Geometry") { X = 1, Y = 9, Width = 40, Height = 8 };
        var lblTurn = new Label("Turn angle:") { X = 0, Y = 0 };
        _turnAngleValue = new Label("-") { X = 14, Y = 0, Width = 18 };
        var lblChord = new Label("Chord brg:") { X = 0, Y = 1 };
        _chordValue = new Label("-") { X = 14, Y = 1, Width = 18 };
        var lblEntry = new Label("Entry tan:") { X = 0, Y = 2 };
        _entryValue = new Label("-") { X = 14, Y = 2, Width = 18 };
        var lblExit = new Label("Exit tan:") { X = 0, Y = 3 };
        _exitValue = new Label("-") { X = 14, Y = 3, Width = 18 };
        var lblHalf = new Label("Half-angle:") { X = 0, Y = 4 };
        _halfAngleValue = new Label("-") { X = 14, Y = 4, Width = 18 };
        resultsFrame.Add(lblTurn, _turnAngleValue, lblChord, _chordValue, lblEntry, _entryValue,
                         lblExit, _exitValue, lblHalf, _halfAngleValue);

        var diagramFrame = new FrameView("Diagram") { X = 42, Y = 9, Width = 28, Height = 14 };
        _diagram = new Label("") { X = 0, Y = 0, Width = Dim.Fill(), Height = Dim.Fill() };
        diagramFrame.Add(_diagram);

        Add(lblInit, _initialField, lblNew, _newField, lblDir, _directionRadio,
            calcBtn, resultsFrame, diagramFrame);

        _vm.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName == nameof(ChordExitViewModel.Result)) Refresh();
        };
    }

    private void Refresh()
    {
        var r = _vm.Result;
        if (r is null)
        {
            _turnAngleValue.Text = "-";
            _chordValue.Text = "-";
            _entryValue.Text = "-";
            _exitValue.Text = "-";
            _halfAngleValue.Text = "-";
            _diagram.Text = "";
            return;
        }
        var ce = r.Value;
        _turnAngleValue.Text = $"{ce.TurnAngleDeg:F2}°";
        _chordValue.Text = ce.IsDegenerate ? "(no turn)" : $"{ce.ChordBearingDeg:F2}°";
        _entryValue.Text = ce.IsDegenerate ? "-" : $"{ce.EntryTangentBearingDeg:F2}°";
        _exitValue.Text = ce.IsDegenerate ? "-" : $"{ce.ExitTangentBearingDeg:F2}°";
        _halfAngleValue.Text = ce.IsDegenerate ? "-" : $"{ce.HalfAngleDeg:F2}°";
        _diagram.Text = AsciiDiagrams.TurnDiagram(ce.TurnAngleDeg, _vm.Direction, width: 26, height: 12);
    }
}
