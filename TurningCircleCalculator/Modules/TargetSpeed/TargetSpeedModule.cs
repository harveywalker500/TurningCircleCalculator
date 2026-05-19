using Terminal.Gui;

namespace TurningCircleCalculator.Modules.TargetSpeed;

/// <summary>
/// Module for computing target speed from observed distance and interval
/// (Guide pg 51).
/// </summary>
public class TargetSpeedModule : ICalculatorModule
{
    public string Id => "target-speed";
    public string Title => "Target Speed";
    public string Category => "Targeting";

    public View CreateView(ModuleContext context) => new TargetSpeedView();
}
