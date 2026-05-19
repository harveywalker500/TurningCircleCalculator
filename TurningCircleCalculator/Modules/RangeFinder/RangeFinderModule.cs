using Terminal.Gui;

namespace TurningCircleCalculator.Modules.RangeFinder;

/// <summary>
/// Module for centiradian-based optical range estimation (Guide pg 42).
/// </summary>
public class RangeFinderModule : ICalculatorModule
{
    public string Id => "range-finder";
    public string Title => "Optical Range (cr)";
    public string Category => "Targeting";

    public View CreateView(ModuleContext context) => new RangeFinderView();
}
