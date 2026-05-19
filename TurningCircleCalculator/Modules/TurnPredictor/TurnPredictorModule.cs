using Terminal.Gui;

namespace TurningCircleCalculator.Modules.TurnPredictor;

/// <summary>
/// Module exposing the quadratic turn-radius predictor (Guide pg 27).
/// </summary>
public class TurnPredictorModule : ICalculatorModule
{
    public string Id => "turn-predictor";
    public string Title => "Turn Radius Predictor";
    public string Category => "Navigation";

    public View CreateView(ModuleContext context) => new TurnPredictorView();
}
