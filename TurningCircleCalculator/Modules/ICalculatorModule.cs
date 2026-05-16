using Terminal.Gui;

namespace TurningCircleCalculator.Modules;

public interface ICalculatorModule
{
    string Id { get; }
    string Title { get; }
    string Category { get; }

    /// <summary>
    /// Creates the view for this module. Called on the UI thread.
    /// </summary>
    View CreateView(ModuleContext context);
}
