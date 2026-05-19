using Terminal.Gui;

namespace TurningCircleCalculator.Modules.RangeFinder;

/// <summary>
/// View for the optical range-finder module.
/// </summary>
public class RangeFinderView : View
{
    private readonly RangeFinderViewModel _vm = new();
    private readonly TextField _heightField;
    private readonly TextField _viewField;
    private readonly Label _rangeMValue;
    private readonly Label _rangeHmValue;
    private readonly Label _errorLabel;

    private const int LabelWidth = 22;

    public RangeFinderView()
    {
        var lblHeight = new Label("Mast height (m):") { X = 1, Y = 0 };
        _heightField = new TextField("") { X = LabelWidth, Y = 0, Width = 10 };
        _heightField.TextChanged += _ =>
        {
            if (double.TryParse(_heightField.Text.ToString(), out var v)) _vm.TrueHeightMeters = v;
        };

        var lblView = new Label("View angle (cr):") { X = 1, Y = 2 };
        _viewField = new TextField("") { X = LabelWidth, Y = 2, Width = 10 };
        _viewField.TextChanged += _ =>
        {
            if (double.TryParse(_viewField.Text.ToString(), out var v)) _vm.ViewAngleCr = v;
        };

        var calcBtn = new Button("Calculate") { X = 1, Y = 4 };
        calcBtn.Clicked += () => _vm.Calculate();

        var resultsFrame = new FrameView("Range") { X = 1, Y = 6, Width = Dim.Fill() - 1, Height = 4 };
        var lblM = new Label("Meters:") { X = 0, Y = 0 };
        _rangeMValue = new Label("-") { X = LabelWidth - 4, Y = 0, Width = 20 };
        var lblHm = new Label("Hectometers:") { X = 0, Y = 1 };
        _rangeHmValue = new Label("-") { X = LabelWidth - 4, Y = 1, Width = 20 };
        resultsFrame.Add(lblM, _rangeMValue, lblHm, _rangeHmValue);

        var hint = new Label("1 cr = 1 m at 100 m. Range = (height / cr) × 100.")
        {
            X = 1, Y = 11, Width = Dim.Fill() - 1
        };

        _errorLabel = new Label("") { X = 1, Y = 13, Width = Dim.Fill() - 1 };

        Add(lblHeight, _heightField, lblView, _viewField, calcBtn, resultsFrame, hint, _errorLabel);

        _vm.PropertyChanged += (_, e) =>
        {
            switch (e.PropertyName)
            {
                case nameof(RangeFinderViewModel.RangeMeters):
                    _rangeMValue.Text = _vm.RangeMeters.HasValue ? $"{_vm.RangeMeters.Value:F2} m" : "-";
                    break;
                case nameof(RangeFinderViewModel.RangeHm):
                    _rangeHmValue.Text = _vm.RangeHm.HasValue ? $"{_vm.RangeHm.Value:F2} hm" : "-";
                    break;
                case nameof(RangeFinderViewModel.Error):
                    _errorLabel.Text = _vm.Error ?? "";
                    break;
            }
        };
    }
}
