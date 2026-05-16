using Terminal.Gui;

namespace TurningCircleCalculator.Modules.Advance;

public class AdvanceView : View
{
    private readonly AdvanceViewModel _vm = new();
    private readonly TextField _radiusField;
    private readonly TextField _courseChangeField;
    private readonly Label _advanceValue;
    private readonly Label _transferValue;
    private readonly Label _errorLabel;

    private const int LabelWidth = 16;

    public AdvanceView()
    {
        var lblRadius = new Label("Turn Radius:") { X = 1, Y = 0 };
        _radiusField = new TextField("") { X = LabelWidth, Y = 0, Width = 12 };
        _radiusField.TextChanged += _ =>
        {
            if (double.TryParse(_radiusField.Text.ToString(), out var v)) _vm.Radius = v;
        };

        var lblChange = new Label("Course Change:") { X = 1, Y = 2 };
        _courseChangeField = new TextField("") { X = LabelWidth, Y = 2, Width = 12 };
        _courseChangeField.TextChanged += _ =>
        {
            if (double.TryParse(_courseChangeField.Text.ToString(), out var v)) _vm.CourseChange = v;
        };

        var calcBtn = new Button("Calculate") { X = 1, Y = 4 };
        calcBtn.Clicked += () => _vm.Calculate();

        var resultsFrame = new FrameView("Results") { X = 1, Y = 6, Width = Dim.Fill() - 1, Height = 4 };
        var lblAdv = new Label("Advance:") { X = 0, Y = 0 };
        _advanceValue = new Label("-") { X = LabelWidth, Y = 0, Width = 20 };
        var lblTrans = new Label("Transfer:") { X = 0, Y = 1 };
        _transferValue = new Label("-") { X = LabelWidth, Y = 1, Width = 20 };
        resultsFrame.Add(lblAdv, _advanceValue, lblTrans, _transferValue);

        _errorLabel = new Label("") { X = 1, Y = 11, Width = Dim.Fill() - 1 };

        Add(lblRadius, _radiusField, lblChange, _courseChangeField, calcBtn, resultsFrame, _errorLabel);

        _vm.PropertyChanged += (_, e) =>
        {
            switch (e.PropertyName)
            {
                case nameof(AdvanceViewModel.Advance):
                    _advanceValue.Text = _vm.Advance.HasValue ? $"{_vm.Advance.Value:F4}" : "-";
                    break;
                case nameof(AdvanceViewModel.Transfer):
                    _transferValue.Text = _vm.Transfer.HasValue ? $"{_vm.Transfer.Value:F4}" : "-";
                    break;
                case nameof(AdvanceViewModel.Error):
                    _errorLabel.Text = _vm.Error ?? "";
                    break;
            }
        };
    }
}
