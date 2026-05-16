using Terminal.Gui;

namespace TurningCircleCalculator.Modules.Advance;

public class AdvanceModule : ICalculatorModule
{
    public string Id => "advance";
    public string Title => "Advance & Transfer";
    public string Category => "Navigation";

    public View CreateView(ModuleContext context) => new AdvanceView();
}
