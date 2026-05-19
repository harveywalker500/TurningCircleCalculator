using Terminal.Gui;

namespace TurningCircleCalculator.Modules.Statistics;

/// <summary>
/// Module for measurement statistics: mean and sample standard deviation
/// (Guide Chapter 6).
/// </summary>
public class StatisticsModule : ICalculatorModule
{
    public string Id => "statistics";
    public string Title => "Measurement Statistics";
    public string Category => "Reference";

    public View CreateView(ModuleContext context) => new StatisticsView();
}
