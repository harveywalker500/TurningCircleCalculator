# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Commands

```bash
# Build (warnings treated as errors via Directory.Build.props)
dotnet build

# Run
dotnet run --project TurningCircleCalculator/TurningCircleCalculator.csproj

# Test all
dotnet test

# Test a single class
dotnet test --filter "FullyQualifiedName~CalculatorTests"
```

## Architecture

A .NET 10.0 TUI application using **Terminal.Gui v1.19.0**, following MVVM + a pluggable module system.

### Startup flow

`Program.cs` runs `TimeSetupDialog` (captures simulated start time), then constructs:
1. `SimulatedClock` (wraps a 1-second `System.Timers.Timer`, exposes `Now`)
2. `JsonPreferencesStore` (JSON file in `%APPDATA%/TurningCircleCalculator/prefs.json`)
3. `ModuleContext` (bundles clock + prefs for modules)
4. `ModuleRegistry` (explicit `.Register()` calls; also supports `ScanAssembly()`)
5. `MainViewModel` (owns TOI countdown; receives clock)
6. `MainView` (hosts everything, polls clock every 500 ms via `AddTimeout`)

### Layer responsibilities

**`Clock/`** — `IClock` (interface for testability) + `SimulatedClock` (production). The VM never holds a timer or calls `Thread.Sleep`; it just reads `Clock.Now`. Tests use `FakeClock` from the test project. `SimulatedClock.ParseOrNow` is `internal` and exposed to tests via `InternalsVisibleTo`.

**`Models/`** — pure static math, no Terminal.Gui or UI deps. Files:
- `Calculator.cs` — `radius = arc / θ`, `chord = 2r·sin(θ/2)`. `IsInfiniteRadius = true` when courses are identical.
- `NavConstants.cs` — nautical mile, knot conversions, quadrant sizes, U-Boat dims, min turn radius.
- `TurnPredictor.cs` — quadratic radius predictor from speed (Krigsmarine pg 27); clamped variant honours the 102m floor.
- `ChordGeometry.cs` — `ComputeChordExit(initialCourse, newCourse, TurnDirection)` returning a `ChordExit` record with chord/tangent bearings and half-angle.
- `Optics.cs` — `RangeMetersFromCentiradian`, `TargetSpeedKnots`, `AngleOnBowFromLength` (asin form, clamped).
- `DeadReckoning.cs` — `DistanceMeters/DistanceNauticalMiles(speedKn, TimeSpan)`.
- `Statistics.cs` — `Mean`, `SampleStandardDeviation` (n-1 form), `Compute` returning a `Stats` record.
- `AsciiDiagrams.cs` — `TurnDiagram(angle, direction, w, h)` rendering a small turn diagram for use in `Label` widgets.

**`ViewModels/`**
- `ViewModelBase` — `INotifyPropertyChanged` with `SetField<T>` (skips raise when value unchanged)
- `RelayCommand` — `ICommand` with optional `canExecute` guard and `RaiseCanExecuteChanged`
- `MainViewModel` — TOI countdown, clock pause/resume, `Tick()` called by the 500 ms poll loop
- `TurningCircleViewModel` — turning circle calculation + turn log (newest entry prepended)
- `TimeSetupViewModel` — startup dialog logic with validation

**`Modules/`**
- `ICalculatorModule` — contract: `Id`, `Title`, `Category`, `CreateView(ModuleContext)`
- `ModuleContext` — shared clock + prefs injected into every module's `CreateView`
- `ModuleRegistry` — explicit `Register()` + `ScanAssembly()` scan; preserves insertion order
- `TurningCircle/` — turning circle (delegates to `TurningCircleViewModel`; also displays chord-exit and ASCII diagram)
- `Advance/` — advance & transfer (Advance = `r·sin θ`, Transfer = `r·(1 − cos θ)`)
- `TurnPredictor/` — predicted radius from speed
- `ChordExit/` — standalone chord-exit calculator + ASCII diagram
- `DeadReckoning/` — live (mark + tick from clock) or manual (typed elapsed) distance
- `RangeFinder/` — centiradian → range
- `TargetSpeed/` — distance + interval → knots / m·s⁻¹ / m·min⁻¹
- `AngleOnBow/` — range + length + view width → AoB
- `Statistics/` — multi-line input → mean and sample standard deviation

The Modules menu is grouped by `Category` (`MainView.BuildModuleMenu` does `GroupBy(m => m.Category)`). Use `"Navigation"`, `"Targeting"`, or `"Reference"` for new modules.

Namespace gotcha: `Modules/TurnPredictor/` shadows the type `Models.TurnPredictor`, so inside that view-model you must fully qualify as `Models.TurnPredictor.PredictRadiusMeters(...)`. Same for `Modules/DeadReckoning/` (which already uses `Models.DeadReckoning.DistanceMeters(...)`).

**`Preferences/`** — `IPreferencesStore` + `JsonPreferencesStore` (key/value, lazy-load, explicit `Save()`)

**`Views/`**
- `MainView` — `Window` with `MenuBar` (File/Time/Modules/Help), clock `FrameView` (top-right), module host `FrameView` (left), `StatusBar` (F1/F5/F10). Module views are swapped into the host frame; `ApplyScheme` recurses into all subviews
- `TimeSetupDialog` — startup `Dialog`, backed by `TimeSetupViewModel`

### Adding a new module (one-file operation)

1. Create a class implementing `ICalculatorModule` (and a `View` subclass).
2. Add one line in `Program.cs`: `.Register(new YourModule())`
3. `MainView` and `MainViewModel` are untouched — the new module appears in the Modules menu automatically.

### Testing

Test project: `TurningCircleCalculator.Tests` (xUnit + FluentAssertions). Structure mirrors the main project. `FakeClock` allows manual `AdvanceSeconds()`. All ViewModel and Model tests are synchronous; `SimulatedClockTests.AdvancesOverTime` sleeps 1.1 s to verify the real timer.

### Key Terminal.Gui v1.19 notes

- `TextField.TextChanged` takes a single `NStack.ustring` argument (ignore it; re-read `TextField.Text`)
- `RadioGroup.SelectedItemChanged` arg has `.SelectedItem` (0-based index)
- UI thread marshaling via `Application.MainLoop.AddTimeout`; background timer callbacks must not touch views directly
