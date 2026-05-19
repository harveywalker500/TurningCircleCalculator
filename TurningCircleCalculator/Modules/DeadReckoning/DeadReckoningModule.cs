using Terminal.Gui;

namespace TurningCircleCalculator.Modules.DeadReckoning;

/// <summary>
/// Module exposing dead-reckoning distance (Guide Chapter 5).
/// </summary>
public class DeadReckoningModule : ICalculatorModule
{
    public string Id => "dead-reckoning";
    public string Title => "Dead Reckoning";
    public string Category => "Navigation";

    public View CreateView(ModuleContext context) => new DeadReckoningView(context);
}
