# Turning Circle Calculator

A terminal-based (TUI) nautical navigation aid for calculating vessel turning circles and Time of Impact (TOI) countdowns. Built with [Terminal.Gui](https://github.com/gui-cs/Terminal.Gui) on .NET 10.

## What it does

Modules are grouped in the menu by category:

### Navigation
- **Turning Circle** — enter an initial course, new course, and the arc distance traveled; get turning radius, chord distance, chord bearing, half-angle, and an ASCII turn diagram.
- **Advance & Transfer** — given a turning radius and course change, calculate how far the vessel advances forward and transfers laterally during the turn.
- **Turn Radius Predictor** — given speed in knots, predict the turning radius from the regression curve in the Krigsmarine guide (page 27). Includes a clamped variant honouring the 102 m floor.
- **Chord Exit Angle** — compute chord and tangent bearings without needing the odometer reading, plus an ASCII diagram.
- **Dead Reckoning** — live mode ticks the distance up from a "Mark Now" event using the simulated clock; manual mode takes a typed elapsed interval. Pause-aware.

### Targeting
- **Optical Range (cr)** — turn a mast height and a centiradian view-angle reading into a range in meters/hectometers (guide page 42).
- **Target Speed** — speed in knots / m·s⁻¹ / m·min⁻¹ from a measured distance and time interval (`mm:ss` or decimal minutes).
- **Angle on Bow** — compute AoB from range, target length, and the centiradian width of the target through optics.

### Reference
- **Measurement Statistics** — paste a list of values (one per line, comma or whitespace separated). Reports count, mean, sample standard deviation, min, and max.

### Cross-module features
- **Simulated clock** — starts from an operator-set time and ticks at 1 × real speed.
- **TOI countdown** — set a Time of Impact; the clock counts down and flashes the screen on arrival.

## Not implemented

These guide chapters require an interactive chart canvas and are intentionally out of scope for the TUI:

- 4-bearing method (fixed and moving) for triangulating a target's course (Chapter 7)
- Pinpoint / running fix from quadrant corners (Chapter 2)
- Visual sweep-path overlay during minimal-radius turns (Chapter 4)
- Free-form chart plotting and timestamp marks

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
Models/                  Pure static math layer:
                           Calculator           — arc-length formula + bearing helpers
                           NavConstants         — knot/NM/quadrant constants
                           TurnPredictor        — quadratic radius regression
                           ChordGeometry        — chord/tangent bearings
                           Optics               — centiradian range, speed, AoB
                           DeadReckoning        — distance from speed × elapsed
                           Statistics           — sample mean / standard deviation
                           AsciiDiagrams        — small ASCII turn diagrams
ViewModels/              ViewModelBase, RelayCommand, MainViewModel, TurningCircleViewModel,
                         TimeSetupViewModel
Modules/
  ICalculatorModule      Module contract
  ModuleContext          Shared dependencies (clock, prefs) injected into every module
  ModuleRegistry         Explicit or scan-based registration, deterministic order
  TurningCircle/         Turning circle (now also chord exit + diagram)
  Advance/               Advance & Transfer
  TurnPredictor/         Predicted radius from speed
  ChordExit/             Standalone chord exit angle
  DeadReckoning/         Live + manual dead-reckoning distance
  RangeFinder/           Centiradian-based optical range
  TargetSpeed/           Target speed from distance and interval
  AngleOnBow/            AoB from optical width and length
  Statistics/            Sample mean / standard deviation
Preferences/             IPreferencesStore + JsonPreferencesStore (~/AppData)
Views/                   MainView (menu, clock panel, module host, status bar), TimeSetupDialog
```
