using Terminal.Gui;
using TurningCircleCalculator.Views;

namespace TurningCircleCalculator;

/// <summary>
/// The entry point class for the Turning Circle Calculator application.
/// </summary>
internal static class Program
{
    /// <summary>
    /// The main entry point for the application.
    /// Initializes the UI and runs the application loop.
    /// </summary>
    /// <param name="args">Command-line arguments.</param>
    private static void Main(string[] args)
    {
        Application.Init();

        var setup = new TimeSetupDialog();
        Application.Run(setup);

        string selectedTime = setup.SelectedTime;

        Application.Run(new MainView(selectedTime));
        Application.Shutdown();
    }
}