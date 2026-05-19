using Terminal.Gui;

namespace TurningCircleCalculator.Modules.TargetSpeed;

/// <summary>
/// View for the Target Speed module. Accepts either a decimal minutes value
/// or an <c>mm:ss</c> interval.
/// </summary>
public class TargetSpeedView : View
{
    private readonly TargetSpeedViewModel _vm = new();
    private readonly TextField _distanceField;
    private readonly TextField _intervalField;
    private readonly Label _knotsValue;
    private readonly Label _mpsValue;
    private readonly Label _mpmValue;
    private readonly Label _errorLabel;

    private const int LabelWidth = 22;

    public TargetSpeedView()
    {
        var lblDist = new Label("Distance (m):") { X = 1, Y = 0 };
        _distanceField = new TextField("") { X = LabelWidth, Y = 0, Width = 12 };
        _distanceField.TextChanged += _ =>
        {
            if (double.TryParse(_distanceField.Text.ToString(), out var v)) _vm.DistanceMeters = v;
        };

        var lblInt = new Label("Interval (min / mm:ss):") { X = 1, Y = 2 };
        _intervalField = new TextField("") { X = LabelWidth, Y = 2, Width = 12 };
        _intervalField.TextChanged += _ =>
        {
            var s = _intervalField.Text.ToString() ?? "";
            if (TryParseInterval(s, out var minutes)) _vm.IntervalMinutes = minutes;
        };

        var calcBtn = new Button("Calculate") { X = 1, Y = 4 };
        calcBtn.Clicked += () => _vm.Calculate();

        var resultsFrame = new FrameView("Speed") { X = 1, Y = 6, Width = Dim.Fill() - 1, Height = 5 };
        var lblK = new Label("Knots:") { X = 0, Y = 0 };
        _knotsValue = new Label("-") { X = LabelWidth - 4, Y = 0, Width = 20 };
        var lblPs = new Label("Meters / second:") { X = 0, Y = 1 };
        _mpsValue = new Label("-") { X = LabelWidth - 4, Y = 1, Width = 20 };
        var lblPm = new Label("Meters / minute:") { X = 0, Y = 2 };
        _mpmValue = new Label("-") { X = LabelWidth - 4, Y = 2, Width = 20 };
        resultsFrame.Add(lblK, _knotsValue, lblPs, _mpsValue, lblPm, _mpmValue);

        _errorLabel = new Label("") { X = 1, Y = 12, Width = Dim.Fill() - 1 };

        Add(lblDist, _distanceField, lblInt, _intervalField, calcBtn, resultsFrame, _errorLabel);

        _vm.PropertyChanged += (_, e) =>
        {
            switch (e.PropertyName)
            {
                case nameof(TargetSpeedViewModel.SpeedKnots):
                    _knotsValue.Text = _vm.SpeedKnots.HasValue ? $"{_vm.SpeedKnots.Value:F2} kn" : "-";
                    break;
                case nameof(TargetSpeedViewModel.SpeedMetersPerSecond):
                    _mpsValue.Text = _vm.SpeedMetersPerSecond.HasValue ? $"{_vm.SpeedMetersPerSecond.Value:F2} m/s" : "-";
                    break;
                case nameof(TargetSpeedViewModel.SpeedMetersPerMinute):
                    _mpmValue.Text = _vm.SpeedMetersPerMinute.HasValue ? $"{_vm.SpeedMetersPerMinute.Value:F2} m/min" : "-";
                    break;
                case nameof(TargetSpeedViewModel.Error):
                    _errorLabel.Text = _vm.Error ?? "";
                    break;
            }
        };
    }

    private static bool TryParseInterval(string s, out double minutes)
    {
        minutes = 0;
        if (string.IsNullOrWhiteSpace(s)) return false;
        if (s.Contains(':'))
        {
            // mm:ss or hh:mm:ss
            if (TimeSpan.TryParse(s, out var ts) || TimeSpan.TryParse("00:" + s, out ts))
            {
                minutes = ts.TotalMinutes;
                return true;
            }
            return false;
        }
        if (double.TryParse(s, out var d))
        {
            minutes = d;
            return true;
        }
        return false;
    }
}
