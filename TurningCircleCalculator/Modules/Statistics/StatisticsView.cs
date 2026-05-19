using Terminal.Gui;

namespace TurningCircleCalculator.Modules.Statistics;

/// <summary>
/// View for the Measurement Statistics module. Accepts one number per line
/// (or comma/space-separated) and displays summary statistics.
/// </summary>
public class StatisticsView : View
{
    private readonly StatisticsViewModel _vm = new();
    private readonly TextView _input;
    private readonly TextField _appendField;
    private readonly Label _countValue;
    private readonly Label _meanValue;
    private readonly Label _stdDevValue;
    private readonly Label _minValue;
    private readonly Label _maxValue;
    private readonly Label _skippedLabel;

    private const int LabelWidth = 18;

    public StatisticsView()
    {
        var lblInput = new Label("Values (one per line):") { X = 1, Y = 0 };
        _input = new TextView { X = 1, Y = 1, Width = 24, Height = 14 };
        _input.Text = "";

        _appendField = new TextField("") { X = 1, Y = 16, Width = 16 };
        var appendBtn = new Button("Append") { X = 18, Y = 16 };
        appendBtn.Clicked += AppendValue;

        var clearBtn = new Button("Clear") { X = 1, Y = 18 };
        clearBtn.Clicked += () =>
        {
            _input.Text = "";
            PushInputToVm();
        };

        var statsFrame = new FrameView("Statistics") { X = 28, Y = 1, Width = Dim.Fill() - 1, Height = 8 };
        var lblCount = new Label("Count:") { X = 0, Y = 0 };
        _countValue = new Label("0") { X = LabelWidth, Y = 0, Width = 16 };
        var lblMean = new Label("Mean:") { X = 0, Y = 1 };
        _meanValue = new Label("-") { X = LabelWidth, Y = 1, Width = 16 };
        var lblStd = new Label("Sample StdDev:") { X = 0, Y = 2 };
        _stdDevValue = new Label("-") { X = LabelWidth, Y = 2, Width = 16 };
        var lblMin = new Label("Min:") { X = 0, Y = 3 };
        _minValue = new Label("-") { X = LabelWidth, Y = 3, Width = 16 };
        var lblMax = new Label("Max:") { X = 0, Y = 4 };
        _maxValue = new Label("-") { X = LabelWidth, Y = 4, Width = 16 };
        statsFrame.Add(lblCount, _countValue, lblMean, _meanValue, lblStd, _stdDevValue,
                       lblMin, _minValue, lblMax, _maxValue);

        _skippedLabel = new Label("") { X = 28, Y = 10, Width = Dim.Fill() - 1 };

        Add(lblInput, _input, _appendField, appendBtn, clearBtn, statsFrame, _skippedLabel);

        // The TextView TextChanged is parameterless in v1.19.
        _input.TextChanged += () => PushInputToVm();

        _vm.PropertyChanged += (_, e) =>
        {
            switch (e.PropertyName)
            {
                case nameof(StatisticsViewModel.Stats):
                case nameof(StatisticsViewModel.SkippedCount):
                    RefreshStats();
                    break;
            }
        };
    }

    private void AppendValue()
    {
        var s = _appendField.Text.ToString();
        if (string.IsNullOrWhiteSpace(s)) return;
        var existing = _input.Text.ToString() ?? "";
        if (existing.Length > 0 && !existing.EndsWith("\n", StringComparison.Ordinal))
            existing += "\n";
        _input.Text = existing + s;
        _appendField.Text = "";
        PushInputToVm();
    }

    private void PushInputToVm() => _vm.RawInput = _input.Text.ToString() ?? "";

    private void RefreshStats()
    {
        var s = _vm.Stats;
        if (s is null)
        {
            _countValue.Text = "0";
            _meanValue.Text = "-";
            _stdDevValue.Text = "-";
            _minValue.Text = "-";
            _maxValue.Text = "-";
        }
        else
        {
            var v = s.Value;
            _countValue.Text = v.Count.ToString();
            _meanValue.Text = $"{v.Mean:F4}";
            _stdDevValue.Text = v.Count < 2 ? "(n<2)" : $"{v.StdDev:F4}";
            _minValue.Text = $"{v.Min:F4}";
            _maxValue.Text = $"{v.Max:F4}";
        }
        _skippedLabel.Text = _vm.SkippedCount > 0 ? $"Skipped {_vm.SkippedCount} invalid token(s)." : "";
    }
}
