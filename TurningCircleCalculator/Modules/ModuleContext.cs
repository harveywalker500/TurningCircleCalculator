using TurningCircleCalculator.Clock;
using TurningCircleCalculator.Preferences;

namespace TurningCircleCalculator.Modules;

public class ModuleContext
{
    public ModuleContext(IClock clock, IPreferencesStore preferences)
    {
        Clock = clock;
        Preferences = preferences;
    }

    public IClock Clock { get; }
    public IPreferencesStore Preferences { get; }
}
