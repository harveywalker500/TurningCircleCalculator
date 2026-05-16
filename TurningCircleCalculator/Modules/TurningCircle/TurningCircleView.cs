using Terminal.Gui;
using TurningCircleCalculator.Modules;
using TurningCircleCalculator.ViewModels;

namespace TurningCircleCalculator.Modules.TurningCircle;

public class TurningCircleView : View
{
    private readonly TurningCircleViewModel _vm;
    private readonly TextField _initialCourseField;
    private readonly TextField _newCourseField;
    private readonly TextField _odometerField;
    private readonly RadioGroup _directionRadio;
    private readonly Label _angleDiffValue;
    private readonly Label _radiusValue;
    private readonly Label _chordValue;
    private readonly TextView _logView;
    private readonly ModuleContext _context;

    private const int LabelWidth = 18;

    public TurningCircleView(ModuleContext context)
    {
        _context = context;
        _vm = new TurningCircleViewModel();

        var lblCourse1 = new Label("Initial Course:") { X = 1, Y = 0 };
        _initialCourseField = new TextField("") { X = LabelWidth, Y = 0, Width = 10 };
        _initialCourseField.TextChanged += _ =>
        {
            if (double.TryParse(_initialCourseField.Text.ToString(), out var v)) _vm.InitialCourse = v;
        };

        var lblCourse2 = new Label("New Course:") { X = 1, Y = 2 };
        _newCourseField = new TextField("") { X = LabelWidth, Y = 2, Width = 10 };
        _newCourseField.TextChanged += _ =>
        {
            if (double.TryParse(_newCourseField.Text.ToString(), out var v)) _vm.NewCourse = v;
        };

        var lblOdo = new Label("Odometer:") { X = 1, Y = 4 };
        _odometerField = new TextField("") { X = LabelWidth, Y = 4, Width = 10 };
        _odometerField.TextChanged += _ =>
        {
            if (double.TryParse(_odometerField.Text.ToString(), out var v)) _vm.Odometer = v;
        };

        var lblDir = new Label("Direction:") { X = 1, Y = 6 };
        _directionRadio = new RadioGroup(new NStack.ustring[] { "Left", "Right" })
        {
            X = LabelWidth, Y = 6, SelectedItem = 0
        };
        _directionRadio.SelectedItemChanged += e =>
            _vm.Direction = e.SelectedItem == 0 ? "Left" : "Right";

        var calcBtn = new Button("Calculate") { X = 1, Y = 9 };
        calcBtn.Clicked += OnCalculate;

        var resultsFrame = new FrameView("Results") { X = 1, Y = 11, Width = Dim.Fill() - 1, Height = 5 };
        var lblAngle = new Label("Angle Diff:") { X = 0, Y = 0 };
        _angleDiffValue = new Label("-") { X = LabelWidth - 5, Y = 0, Width = 20 };
        var lblRadius = new Label("Radius:") { X = 0, Y = 1 };
        _radiusValue = new Label("-") { X = LabelWidth - 5, Y = 1, Width = 20 };
        var lblChord = new Label("Chord:") { X = 0, Y = 2 };
        _chordValue = new Label("-") { X = LabelWidth - 5, Y = 2, Width = 20 };
        resultsFrame.Add(lblAngle, _angleDiffValue, lblRadius, _radiusValue, lblChord, _chordValue);

        var logFrame = new FrameView("Turn Log") { X = 1, Y = 17, Width = Dim.Fill() - 1, Height = Dim.Fill() - 1 };
        _logView = new TextView { X = 0, Y = 0, Width = Dim.Fill(), Height = Dim.Fill(), ReadOnly = true };
        logFrame.Add(_logView);

        Add(lblCourse1, _initialCourseField, lblCourse2, _newCourseField,
            lblOdo, _odometerField, lblDir, _directionRadio,
            calcBtn, resultsFrame, logFrame);

        _vm.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName == nameof(TurningCircleViewModel.TurnLog))
                _logView.Text = _vm.TurnLog;
            if (e.PropertyName == nameof(TurningCircleViewModel.LastResult) && _vm.LastResult != null)
            {
                var r = _vm.LastResult;
                _angleDiffValue.Text = $"{r.AngleDifference:F4}°";
                _radiusValue.Text = r.IsInfiniteRadius ? "Infinite" : r.Radius.ToString("F4");
                _chordValue.Text = r.IsInfiniteRadius ? "-" : r.Chord.ToString("F4");
            }
        };
    }

    private void OnCalculate()
    {
        _vm.Calculate(_context.Clock.Now);
        _initialCourseField.Text = _vm.InitialCourse.ToString("F2");
        _newCourseField.Text = "";
        _odometerField.Text = "";
        _newCourseField.SetFocus();
    }
}
