# Turning Circle Calculator

A terminal-based (TUI) nautical navigation aid for calculating vessel turning circles and Time of Impact (TOI) countdowns. Built with [Terminal.Gui](https://github.com/gui-cs/Terminal.Gui) on .NET 10.

## What it does

- **Turning Circle** — enter an initial course, new course, and the arc distance traveled; get back turning radius and chord distance.
- **Advance & Transfer** — given a turning radius and course change, calculate how far the vessel advances forward and transfers laterally during the turn.
- **Simulated clock** — starts from an operator-set time and ticks at 1 × real speed, keeping you synced to an operation's timeline rather than wall time.
- **TOI countdown** — set a Time of Impact; the clock counts down and flashes the screen on arrival.
- **Night mode** — press the Night Mode button or use the menu to switch to a red-on-black color scheme.

## Build and run

Requires .NET 10 SDK.

```bash
dotnet build
dotnet run --project TurningCircleCalculator/TurningCircleCalculator.csproj
```

On first launch you are prompted to set the simulated start time. Press **Cancel** to default to the current wall time.

## Run tests

```bash
dotnet test
```

## Module system

Each calculator mode is an `ICalculatorModule`:

```csharp
public interface ICalculatorModule
{
    string Id       { get; }
    string Title    { get; }
    string Category { get; }
    View CreateView(ModuleContext context);
}
```

Adding a new module requires:

1. A class implementing `ICalculatorModule` (providing `Id`, `Title`, `Category`, and a `CreateView` factory).
2. A `View` subclass for the UI.
3. Optionally a `ViewModelBase` subclass for its logic.
4. **One line** in `Program.cs`:
   ```csharp
   .Register(new YourNewModule())
   ```

`MainView` and `MainViewModel` are not modified. The module's view is swapped into the module host frame at runtime; modules are listed under the **Modules** menu.

`ModuleContext` provides shared cross-cutting dependencies (the clock and the preferences store) to every module.

## SDK version note

This project targets `net10.0`. .NET 10 is a Standard-Term Support (STS) release; if you need a Long-Term Support target, retargeting to `net8.0` requires only changing `<TargetFramework>` in `Directory.Build.props`.

## Architecture overview

```
Clock/                   IClock interface + SimulatedClock implementation
Models/                  Pure static Calculator (arc-length formula) + CalculationResult
ViewModels/              ViewModelBase, RelayCommand, MainViewModel, TurningCircleViewModel, TimeSetupViewModel
Modules/
  ICalculatorModule      Module contract
  ModuleContext          Shared dependencies (clock, prefs) injected into every module
  ModuleRegistry         Explicit or scan-based registration, deterministic order
  TurningCircle/         Turning circle module (view + module class)
  Advance/               Advance & Transfer module (view + viewmodel + module class)
Preferences/             IPreferencesStore + JsonPreferencesStore (~/AppData)
Views/                   MainView (menu, clock panel, module host, status bar), TimeSetupDialog
```
