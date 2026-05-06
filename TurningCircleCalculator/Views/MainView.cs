using Terminal.Gui;
using TurningCircleCalculator.Models;
using TurningCircleCalculator.ViewModels;

namespace TurningCircleCalculator.Views;

public class MainView : Window
{
    private readonly MainViewModel _viewModel;
    private readonly TextField _initialCourseField;
    private readonly TextField _newCourseField;
    private readonly TextField _odometerField;
    private readonly RadioGroup _directionRadio;
    private readonly Label _angleDiffValue;
    private readonly Label _radiusValue;
    private readonly Label _chordValue;
    private readonly Label _currentTimeLabel;
    private readonly TextField _initialTimeField;
    private readonly TextView _resultsView;
    private readonly TimeController _timeController;
    
    private readonly Label _toiCountdownLabel;
    private readonly Label _toiDisplayLabel;
    private readonly TextField _toiTimeField;

    private ColorScheme _currentScheme;
    private readonly ColorScheme _nauticalScheme;
    private readonly ColorScheme _nightScheme;
    private readonly ColorScheme _flashScheme;

    public MainView(string? initialTime = null)
    {
        _viewModel = new MainViewModel();
        Title = "Turning Circle Calculator (MVVM)";

        // Define Color Schemes
        _nauticalScheme = new ColorScheme()
        {
            Normal = Terminal.Gui.Attribute.Make(Color.White, Color.Blue),
            Focus = Terminal.Gui.Attribute.Make(Color.BrightYellow, Color.Cyan),
            HotNormal = Terminal.Gui.Attribute.Make(Color.BrightCyan, Color.Blue),
            HotFocus = Terminal.Gui.Attribute.Make(Color.BrightYellow, Color.Cyan),
            Disabled = Terminal.Gui.Attribute.Make(Color.Gray, Color.Blue)
        };

        _nightScheme = new ColorScheme()
        {
            Normal = Terminal.Gui.Attribute.Make(Color.BrightRed, Color.Black),
            Focus = Terminal.Gui.Attribute.Make(Color.White, Color.DarkGray),
            HotNormal = Terminal.Gui.Attribute.Make(Color.Red, Color.Black),
            HotFocus = Terminal.Gui.Attribute.Make(Color.White, Color.DarkGray),
            Disabled = Terminal.Gui.Attribute.Make(Color.DarkGray, Color.Black)
        };

        _flashScheme = new ColorScheme()
        {
            Normal = Terminal.Gui.Attribute.Make(Color.Black, Color.BrightYellow),
            Focus = Terminal.Gui.Attribute.Make(Color.Black, Color.BrightYellow),
            HotNormal = Terminal.Gui.Attribute.Make(Color.Black, Color.BrightYellow),
            HotFocus = Terminal.Gui.Attribute.Make(Color.Black, Color.BrightYellow),
            Disabled = Terminal.Gui.Attribute.Make(Color.Black, Color.BrightYellow)
        };

        _currentScheme = _nauticalScheme;
        ColorScheme = _currentScheme;

        var leftColumnWidth = 20;

        var lblInitial = new Label("Initial Course:") { X = 1, Y = 1 };
        _initialCourseField = new TextField("") { X = leftColumnWidth, Y = 1, Width = 10 };
        _initialCourseField.TextChanged += (e) => {
            if (double.TryParse(_initialCourseField.Text.ToString(), out double val)) _viewModel.InitialCourse = val;
        };

        var lblNew = new Label("New Course:") { X = 1, Y = 3 };
        _newCourseField = new TextField("") { X = leftColumnWidth, Y = 3, Width = 10 };
        _newCourseField.TextChanged += (e) => {
            if (double.TryParse(_newCourseField.Text.ToString(), out double val)) _viewModel.NewCourse = val;
        };

        var lblOdometer = new Label("Odometer:") { X = 1, Y = 5 };
        _odometerField = new TextField("") { X = leftColumnWidth, Y = 5, Width = 10 };
        _odometerField.TextChanged += (e) => {
            if (double.TryParse(_odometerField.Text.ToString(), out double val)) _viewModel.Odometer = val;
        };

        var lblDirection = new Label("Direction:") { X = 1, Y = 7 };
        _directionRadio = new RadioGroup(new NStack.ustring[] { "Left", "Right" })
        {
            X = leftColumnWidth,
            Y = 7,
            SelectedItem = 0
        };
        _directionRadio.SelectedItemChanged += (e) => {
            _viewModel.Direction = e.SelectedItem == 0 ? "Left" : "Right";
        };

        var lblInitialTime = new Label("Initial Time:") { X = 1, Y = 9 };
        _initialTimeField = new TextField(initialTime ?? DateTime.Now.ToString("HH:mm:ss")) { X = leftColumnWidth, Y = 9, Width = 12 };

        var calcBtn = new Button("Calculate")
        {
            X = Pos.Left(lblInitialTime),
            Y = 11
        };
        calcBtn.Clicked += () => {
            _viewModel.Calculate();
            // Sync fields back if needed (though InitialCourse is updated in VM)
            _initialCourseField.Text = _viewModel.InitialCourse.ToString("F2");
            _newCourseField.Text = "";
            _odometerField.Text = "";
            _newCourseField.SetFocus();
        };

        var setTimeBtn = new Button("Set Time")
        {
            X = Pos.Right(calcBtn) + 2,
            Y = 11
        };
        setTimeBtn.Clicked += OnSetTime;

        // TOI Controls
        var lblToi = new Label("Set TOI:") { X = 1, Y = 13 };
        _toiTimeField = new TextField("") { X = leftColumnWidth, Y = 13, Width = 12 };
        var setToiBtn = new Button("Set TOI") { X = Pos.Right(_toiTimeField) + 1, Y = 13 };
        setToiBtn.Clicked += () => {
            if (DateTime.TryParse(_toiTimeField.Text.ToString(), out DateTime toi)) {
                // Ensure it's today's date or similar logic
                var now = _viewModel.CurrentTime;
                var target = new DateTime(now.Year, now.Month, now.Day, toi.Hour, toi.Minute, toi.Second);
                if (target < now) target = target.AddDays(1);
                _viewModel.ToiTime = target;
            } else {
                MessageBox.ErrorQuery("Error", "Invalid TOI format", "Ok");
            }
        };

        var resultsFrame = new FrameView("Results")
        {
            X = 1,
            Y = 15,
            Width = Dim.Fill() - 1,
            Height = 6
        };

        var lblAngleDiff = new Label("Angle Difference:") { X = 1, Y = 0 };
        _angleDiffValue = new Label("-") { X = leftColumnWidth, Y = 0, Width = 20 };

        var lblRadius = new Label("Circle Radius:") { X = 1, Y = 1 };
        _radiusValue = new Label("-") { X = leftColumnWidth, Y = 1, Width = 20 };

        var lblChord = new Label("Chord Distance:") { X = 1, Y = 2 };
        _chordValue = new Label("-") { X = leftColumnWidth, Y = 2, Width = 20 };

        resultsFrame.Add(lblAngleDiff, _angleDiffValue, lblRadius, _radiusValue, lblChord, _chordValue);

        var timeFrame = new FrameView("Simulated Time")
        {
            X = Pos.Right(resultsFrame) - 25,
            Y = 1,
            Width = 24,
            Height = 5
        };
        _currentTimeLabel = new Label("00:00:00")
        {
            X = Pos.Center(),
            Y = 0
        };
        _toiCountdownLabel = new Label("TOI: Not Set")
        {
            X = Pos.Center(),
            Y = 1
        };
        _toiDisplayLabel = new Label("TOI @ --:--:--")
        {
            X = Pos.Center(),
            Y = 2
        };
        timeFrame.Add(_currentTimeLabel, _toiCountdownLabel, _toiDisplayLabel);

        _timeController = new TimeController(initialTime ?? DateTime.Now.ToString("HH:mm:ss"));
        _viewModel.CurrentTime = _timeController.Time;

        Application.MainLoop.AddTimeout(TimeSpan.FromMilliseconds(500), (loop) => {
            _viewModel.CurrentTime = _timeController.Time;
            _currentTimeLabel.Text = _viewModel.CurrentTime.ToString("HH:mm:ss");
            _toiCountdownLabel.Text = _viewModel.ToiCountdown;
            _toiDisplayLabel.Text = _viewModel.ToiDisplayTime;
            return true;
        });

        var logFrame = new FrameView("Turn Log")
        {
            X = 1,
            Y = 22,
            Width = Dim.Fill() - 1,
            Height = Dim.Fill() - 1
        };

        _resultsView = new TextView()
        {
            X = 0,
            Y = 0,
            Width = Dim.Fill(),
            Height = Dim.Fill(),
            ReadOnly = true
        };
        logFrame.Add(_resultsView);

        // Color Scheme Toggle
        var nightModeBtn = new Button("Toggle Night Mode")
        {
            X = Pos.Right(setTimeBtn) + 2,
            Y = 11
        };
        nightModeBtn.Clicked += () => {
            _currentScheme = _currentScheme == _nauticalScheme ? _nightScheme : _nauticalScheme;
            ApplyScheme(this, _currentScheme);
        };

        Add(lblInitial, _initialCourseField, lblNew, _newCourseField, lblOdometer, _odometerField, lblDirection, _directionRadio, 
            lblInitialTime, _initialTimeField, calcBtn, setTimeBtn, nightModeBtn, lblToi, _toiTimeField, setToiBtn,
            resultsFrame, timeFrame, logFrame);

        // ViewModel event handlers
        _viewModel.PropertyChanged += (s, e) => {
            if (e.PropertyName == nameof(MainViewModel.TurnLog)) _resultsView.Text = _viewModel.TurnLog;
            if (e.PropertyName == nameof(MainViewModel.LastResult)) {
                var res = _viewModel.LastResult;
                _angleDiffValue.Text = $"{res.AngleDifference:F4}°";
                _radiusValue.Text = res.IsInfiniteRadius ? "Infinite" : res.Radius.ToString("F4");
                _chordValue.Text = res.IsInfiniteRadius ? "-" : res.Chord.ToString("F4");
            }
        };

        _viewModel.OnImpact += () => {
            FlashScreen();
        };
    }

    private void ApplyScheme(View view, ColorScheme scheme)
    {
        view.ColorScheme = scheme;
        foreach (var subview in view.Subviews)
        {
            ApplyScheme(subview, scheme);
        }
    }

    private void FlashScreen()
    {
        var originalScheme = _currentScheme;
        ApplyScheme(this, _flashScheme);
        Application.MainLoop.AddTimeout(TimeSpan.FromMilliseconds(500), (loop) => {
            ApplyScheme(this, originalScheme);
            return false;
        });
    }

    private void OnSetTime()
    {
        _timeController.InitialiseTime(_initialTimeField.Text.ToString() ?? string.Empty);
    }
}
