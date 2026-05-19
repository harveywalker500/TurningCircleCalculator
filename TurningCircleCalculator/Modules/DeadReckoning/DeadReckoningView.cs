using Terminal.Gui;

namespace TurningCircleCalculator.Modules.DeadReckoning;

/// <summary>
/// View for the Dead Reckoning module. Subscribes to the application's main
/// loop so the live mode advances with the simulated clock.
/// </summary>
public class DeadReckoningView : View
{
    private readonly DeadReckoningViewModel _vm = new();
    private readonly ModuleContext _context;
    private readonly TextField _speedField;
    private readonly TextField _courseField;
    private readonly TextField _elapsedField;
    private readonly RadioGroup _modeRadio;
    private readonly Button _markBtn;
    private readonly Button _resetBtn;
    private readonly Label _markLabel;
    private readonly Label _elapsedLabel;
    private readonly Label _distanceMValue;
    private readonly Label _distanceNmValue;
    private readonly Label _errorLabel;

    private const int LabelWidth = 18;

    public DeadReckoningView(ModuleContext context)
    {
        _context = context;

        var lblSpeed = new Label("Speed (kn):") { X = 1, Y = 0 };
        _speedField = new TextField("") { X = LabelWidth, Y = 0, Width = 10 };
        _speedField.TextChanged += _ =>
        {
            if (double.TryParse(_speedField.Text.ToString(), out var v)) _vm.SpeedKnots = v;
        };

        var lblCourse = new Label("Course (°):") { X = 1, Y = 2 };
        _courseField = new TextField("") { X = LabelWidth, Y = 2, Width = 10 };
        _courseField.TextChanged += _ =>
        {
            if (double.TryParse(_courseField.Text.ToString(), out var v)) _vm.CourseDeg = v;
        };

        var lblMode = new Label("Mode:") { X = 1, Y = 4 };
        _modeRadio = new RadioGroup(new NStack.ustring[] { "Live from mark", "Manual elapsed" })
        {
            X = LabelWidth, Y = 4, SelectedItem = 0
        };
        _modeRadio.SelectedItemChanged += e =>
            _vm.Mode = e.SelectedItem == 0 ? DrMode.Live : DrMode.Manual;

        _markBtn = new Button("Mark Now") { X = 1, Y = 7 };
        _markBtn.Clicked += () => _vm.Mark(_context.Clock.Now);
        _resetBtn = new Button("Reset Mark") { X = 14, Y = 7 };
        _resetBtn.Clicked += () => _vm.ResetMark();

        var lblElapsed = new Label("Elapsed (hh:mm:ss):") { X = 1, Y = 9 };
        _elapsedField = new TextField("") { X = LabelWidth + 3, Y = 9, Width = 12 };
        _elapsedField.TextChanged += _ =>
        {
            if (TimeSpan.TryParse(_elapsedField.Text.ToString(), out var ts)) _vm.ManualElapsed = ts;
        };

        var statusFrame = new FrameView("Status") { X = 1, Y = 11, Width = Dim.Fill() - 1, Height = 4 };
        _markLabel = new Label("Mark: (not set)") { X = 0, Y = 0, Width = Dim.Fill() };
        _elapsedLabel = new Label("Elapsed: 00:00:00") { X = 0, Y = 1, Width = Dim.Fill() };
        statusFrame.Add(_markLabel, _elapsedLabel);

        var resultsFrame = new FrameView("Distance") { X = 1, Y = 15, Width = Dim.Fill() - 1, Height = 4 };
        var lblM = new Label("Meters:") { X = 0, Y = 0 };
        _distanceMValue = new Label("0.00") { X = LabelWidth - 4, Y = 0, Width = 20 };
        var lblNm = new Label("Nautical miles:") { X = 0, Y = 1 };
        _distanceNmValue = new Label("0.0000") { X = LabelWidth - 4, Y = 1, Width = 20 };
        resultsFrame.Add(lblM, _distanceMValue, lblNm, _distanceNmValue);

        _errorLabel = new Label("") { X = 1, Y = 20, Width = Dim.Fill() - 1 };

        Add(lblSpeed, _speedField, lblCourse, _courseField, lblMode, _modeRadio,
            _markBtn, _resetBtn, lblElapsed, _elapsedField,
            statusFrame, resultsFrame, _errorLabel);

        _vm.PropertyChanged += (_, e) => RefreshFromVm(e.PropertyName);

        Application.MainLoop.AddTimeout(TimeSpan.FromMilliseconds(500), _ =>
        {
            _vm.Tick(_context.Clock.Now);
            return true;
        });

        RefreshFromVm(null);
    }

    private void RefreshFromVm(string? property)
    {
        _markLabel.Text = _vm.MarkTime.HasValue
            ? $"Mark: {_vm.MarkTime.Value:HH:mm:ss}"
            : "Mark: (not set)";
        _elapsedLabel.Text = $"Elapsed: {_vm.LiveElapsed:hh\\:mm\\:ss}";
        _distanceMValue.Text = $"{_vm.DistanceMeters:F2}";
        _distanceNmValue.Text = $"{_vm.DistanceNm:F4}";
        _errorLabel.Text = _vm.Error ?? "";
    }
}
