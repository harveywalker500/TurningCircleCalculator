using Terminal.Gui;

namespace TurningCircleCalculator.Modules.ChordExit;

/// <summary>
/// Module exposing chord-exit geometry (Guide pp. 24-26).
/// </summary>
public class ChordExitModule : ICalculatorModule
{
    public string Id => "chord-exit";
    public string Title => "Chord Exit Angle";
    public string Category => "Navigation";

    public View CreateView(ModuleContext context) => new ChordExitView();
}
