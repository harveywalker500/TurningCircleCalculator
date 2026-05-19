using Terminal.Gui;

namespace TurningCircleCalculator.Modules.AngleOnBow;

/// <summary>
/// Module for Angle on Bow from optics (Guide pg 56).
/// </summary>
public class AngleOnBowModule : ICalculatorModule
{
    public string Id => "angle-on-bow";
    public string Title => "Angle on Bow";
    public string Category => "Targeting";

    public View CreateView(ModuleContext context) => new AngleOnBowView();
}
