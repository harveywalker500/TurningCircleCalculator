using System.Globalization;
using TurningCircleCalculator.ViewModels;

namespace TurningCircleCalculator.Modules.Statistics;

/// <summary>
/// Backing ViewModel for the Measurement Statistics module.
/// </summary>
public class StatisticsViewModel : ViewModelBase
{
    private string _rawInput = string.Empty;
    private Models.Stats? _stats;
    private int _skippedCount;

    public string RawInput
    {
        get => _rawInput;
        set { SetField(ref _rawInput, value); Recompute(); }
    }

    public Models.Stats? Stats
    {
        get => _stats;
        private set => SetField(ref _stats, value);
    }

    public int SkippedCount
    {
        get => _skippedCount;
        private set => SetField(ref _skippedCount, value);
    }

    public void Recompute()
    {
        var (values, skipped) = Parse(_rawInput);
        SkippedCount = skipped;
        Stats = values.Count == 0 ? null : Models.Statistics.Compute(values);
    }

    private static (List<double> Values, int Skipped) Parse(string text)
    {
        var values = new List<double>();
        int skipped = 0;
        if (string.IsNullOrWhiteSpace(text)) return (values, 0);

        var tokens = text.Split(new[] { '\n', '\r', ',', ' ', '\t', ';' }, StringSplitOptions.RemoveEmptyEntries);
        foreach (var tok in tokens)
        {
            if (double.TryParse(tok, NumberStyles.Float, CultureInfo.InvariantCulture, out var v))
                values.Add(v);
            else
                skipped++;
        }
        return (values, skipped);
    }
}
