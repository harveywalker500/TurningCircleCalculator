using Terminal.Gui;

namespace TurningCircleCalculator.Modules.TurningCircle;

public class TurningCircleModule : ICalculatorModule
{
    public string Id => "turning-circle";
    public string Title => "Turning Circle";
    public string Category => "Navigation";

    public View CreateView(ModuleContext context) => new TurningCircleView(context);
}
