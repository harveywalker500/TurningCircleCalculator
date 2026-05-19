using Terminal.Gui;

namespace TurningCircleCalculator.Modules.AngleOnBow;

/// <summary>
/// View for the Angle on Bow module.
/// </summary>
public class AngleOnBowView : View
{
    private readonly AngleOnBowViewModel _vm = new();
    private readonly TextField _rangeField;
    private readonly TextField _viewField;
    private readonly TextField _lengthField;
    private readonly Label _aobValue;
    private readonly Label _errorLabel;

    private const int LabelWidth = 22;

    public AngleOnBowView()
    {
        var lblRange = new Label("Range to target (m):") { X = 1, Y = 0 };
        _rangeField = new TextField("") { X = LabelWidth, Y = 0, Width = 12 };
        _rangeField.TextChanged += _ =>
        {
            if (double.TryParse(_rangeField.Text.ToString(), out var v)) _vm.RangeMeters = v;
        };

        var lblView = new Label("Width view (cr):") { X = 1, Y = 2 };
        _viewField = new TextField("") { X = LabelWidth, Y = 2, Width = 12 };
        _viewField.TextChanged += _ =>
        {
            if (double.TryParse(_viewField.Text.ToString(), out var v)) _vm.ViewAngleCr = v;
        };

        var lblLen = new Label("True length (m):") { X = 1, Y = 4 };
        _lengthField = new TextField("") { X = LabelWidth, Y = 4, Width = 12 };
        _lengthField.TextChanged += _ =>
        {
            if (double.TryParse(_lengthField.Text.ToString(), out var v)) _vm.TrueLengthMeters = v;
        };

        var calcBtn = new Button("Calculate") { X = 1, Y = 6 };
        calcBtn.Clicked += () => _vm.Calculate();

        var resultsFrame = new FrameView("Result") { X = 1, Y = 8, Width = Dim.Fill() - 1, Height = 3 };
        var lblAob = new Label("Angle on Bow:") { X = 0, Y = 0 };
        _aobValue = new Label("-") { X = LabelWidth - 4, Y = 0, Width = 20 };
        resultsFrame.Add(lblAob, _aobValue);

        var hint = new Label("AoB = asin((Range × cr) / (100 × Length)). 0° = bow-on, 90° = broadside.")
        {
            X = 1, Y = 12, Width = Dim.Fill() - 1
        };

        _errorLabel = new Label("") { X = 1, Y = 14, Width = Dim.Fill() - 1 };

        Add(lblRange, _rangeField, lblView, _viewField, lblLen, _lengthField,
            calcBtn, resultsFrame, hint, _errorLabel);

        _vm.PropertyChanged += (_, e) =>
        {
            switch (e.PropertyName)
            {
                case nameof(AngleOnBowViewModel.AobDeg):
                    _aobValue.Text = _vm.AobDeg.HasValue ? $"{_vm.AobDeg.Value:F2}°" : "-";
                    break;
                case nameof(AngleOnBowViewModel.Error):
                    _errorLabel.Text = _vm.Error ?? "";
                    break;
            }
        };
    }
}
