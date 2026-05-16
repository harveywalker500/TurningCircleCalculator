using Terminal.Gui;
using TurningCircleCalculator.Clock;
using TurningCircleCalculator.Modules;
using TurningCircleCalculator.Modules.Advance;
using TurningCircleCalculator.Modules.TurningCircle;
using TurningCircleCalculator.Preferences;
using TurningCircleCalculator.ViewModels;
using TurningCircleCalculator.Views;

namespace TurningCircleCalculator;

internal static class Program
{
    private static void Main(string[] args)
    {
        Application.Init();

        // Time setup
        var setupDialog = new TimeSetupDialog();
        Application.Run(setupDialog);
        var selectedTime = setupDialog.SelectedTime;

        // Infrastructure
        var clock = new SimulatedClock(selectedTime);
        var prefPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "TurningCircleCalculator", "prefs.json");
        var prefs = new JsonPreferencesStore(prefPath);
        var context = new ModuleContext(clock, prefs);

        // Module registration — adding a new module is one line here
        var registry = new ModuleRegistry()
            .Register(new TurningCircleModule())
            .Register(new AdvanceModule());

        // Launch
        var vm = new MainViewModel(clock);
        Application.Run(new MainView(vm, registry, context));

        clock.Dispose();
        Application.Shutdown();
    }
}
