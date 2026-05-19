using Terminal.Gui;
using TurningCircleCalculator.Models;

namespace TurningCircleCalculator.Modules.TurnPredictor;

/// <summary>
/// View for the Turn Radius Predictor module.
/// </summary>
public class TurnPredictorView : View
{
    private readonly TurnPredictorViewModel _vm = new();
    private readonly TextField _speedField;
    private readonly Label _radiusValue;
    private readonly Label _clampedValue;
    private readonly Label _clampNote;
    private readonly Label _errorLabel;

    private const int LabelWidth = 22;

    public TurnPredictorView()
    {
        var lblSpeed = new Label("Speed (kn):") { X = 1, Y = 0 };
        _speedField = new TextField("") { X = LabelWidth, Y = 0, Width = 10 };
        _speedField.TextChanged += _ =>
        {
            if (double.TryParse(_speedField.Text.ToString(), out var v)) _vm.SpeedKnots = v;
        };

        var calcBtn = new Button("Predict") { X = 1, Y = 2 };
        calcBtn.Clicked += () => _vm.Calculate();

        var resultsFrame = new FrameView("Predicted Radius") { X = 1, Y = 4, Width = Dim.Fill() - 1, Height = 6 };
        var lblRaw = new Label("Raw (quadratic):") { X = 0, Y = 0 };
        _radiusValue = new Label("-") { X = LabelWidth - 4, Y = 0, Width = 20 };
        var lblClamped = new Label("Clamped (≥ floor):") { X = 0, Y = 1 };
        _clampedValue = new Label("-") { X = LabelWidth - 4, Y = 1, Width = 20 };
        _clampNote = new Label("") { X = 0, Y = 3, Width = Dim.Fill() };
        resultsFrame.Add(lblRaw, _radiusValue, lblClamped, _clampedValue, _clampNote);

        var hint = new Label($"Floor: {NavConstants.MinTurnRadiusMeters:F0} m at ≈ {NavConstants.MinTurnRadiusSpeedKnots:F0} kn (full rudder).")
        {
            X = 1, Y = 11, Width = Dim.Fill() - 1
        };

        _errorLabel = new Label("") { X = 1, Y = 13, Width = Dim.Fill() - 1 };

        Add(lblSpeed, _speedField, calcBtn, resultsFrame, hint, _errorLabel);

        _vm.PropertyChanged += (_, e) =>
        {
            switch (e.PropertyName)
            {
                case nameof(TurnPredictorViewModel.RadiusMeters):
                    _radiusValue.Text = _vm.RadiusMeters.HasValue ? $"{_vm.RadiusMeters.Value:F2} m" : "-";
                    break;
                case nameof(TurnPredictorViewModel.RadiusClampedMeters):
                    _clampedValue.Text = _vm.RadiusClampedMeters.HasValue ? $"{_vm.RadiusClampedMeters.Value:F2} m" : "-";
                    break;
                case nameof(TurnPredictorViewModel.IsClamped):
                    _clampNote.Text = _vm.IsClamped
                        ? "↑ Quadratic fell below the U-Boat floor; clamped."
                        : "";
                    break;
                case nameof(TurnPredictorViewModel.Error):
                    _errorLabel.Text = _vm.Error ?? "";
                    break;
            }
        };
    }
}
