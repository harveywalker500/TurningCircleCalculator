using Terminal.Gui;
using TurningCircleCalculator.Modules;
using TurningCircleCalculator.ViewModels;

namespace TurningCircleCalculator.Views;

public class MainView : Window
{
    private readonly MainViewModel _vm;
    private readonly ModuleRegistry _registry;
    private readonly ModuleContext _context;

    private readonly Label _currentTimeLabel;
    private readonly Label _toiCountdownLabel;
    private readonly Label _toiDisplayLabel;
    private readonly TextField _toiTimeField;
    private readonly FrameView _moduleHost;
    private readonly StatusBar _statusBar;

    private ICalculatorModule? _activeModule;
    private View? _activeModuleView;

    private ColorScheme _activeScheme;
    private readonly ColorScheme _nauticalScheme;
    private readonly ColorScheme _nightScheme;
    private readonly ColorScheme _flashScheme;

    public MainView(MainViewModel vm, ModuleRegistry registry, ModuleContext context)
    {
        _vm = vm;
        _registry = registry;
        _context = context;

        _nauticalScheme = MakeScheme(Color.White, Color.Blue, Color.BrightYellow, Color.Cyan, Color.BrightCyan, Color.Gray);
        _nightScheme = MakeScheme(Color.BrightRed, Color.Black, Color.White, Color.DarkGray, Color.Red, Color.DarkGray);
        _flashScheme = new ColorScheme
        {
            Normal = Terminal.Gui.Attribute.Make(Color.Black, Color.BrightYellow),
            Focus = Terminal.Gui.Attribute.Make(Color.Black, Color.BrightYellow),
            HotNormal = Terminal.Gui.Attribute.Make(Color.Black, Color.BrightYellow),
            HotFocus = Terminal.Gui.Attribute.Make(Color.Black, Color.BrightYellow),
            Disabled = Terminal.Gui.Attribute.Make(Color.Black, Color.BrightYellow)
        };
        _activeScheme = _nauticalScheme;
        ColorScheme = _activeScheme;

        Title = "Turning Circle Calculator";

        // ── MenuBar ────────────────────────────────────────────────────────────
        var menuBar = new MenuBar(new[]
        {
            new MenuBarItem("_File", new[]
            {
                new MenuItem("_Quit", "F10", () => Application.RequestStop())
            }),
            new MenuBarItem("_Time", new[]
            {
                new MenuItem("_Set Time…", "", OnSetTime),
                new MenuItem("_Pause / Resume", "F5", OnTogglePause)
            }),
            BuildModuleMenu(),
            new MenuBarItem("_Help", new[]
            {
                new MenuItem("_About", "", () =>
                    MessageBox.Query("About", "Turning Circle Calculator\nA nautical navigation aid.", "OK"))
            })
        });

        // ── Clock frame (top-right) ─────────────────────────────────────────────
        var clockFrame = new FrameView("Clock")
        {
            X = Pos.AnchorEnd(26), Y = 1, Width = 25, Height = 5
        };
        _currentTimeLabel = new Label("--:--:--") { X = Pos.Center(), Y = 0 };
        _toiCountdownLabel = new Label("TOI: Not Set") { X = Pos.Center(), Y = 1 };
        _toiDisplayLabel = new Label("TOI @ --:--:--") { X = Pos.Center(), Y = 2 };

        var lblToiInput = new Label("Set TOI:") { X = 0, Y = 3 };
        _toiTimeField = new TextField("") { X = 9, Y = 3, Width = 10 };
        var setToiBtn = new Button("Set") { X = Pos.Right(_toiTimeField) + 1, Y = 3 };
        setToiBtn.Clicked += OnSetToi;
        clockFrame.Add(_currentTimeLabel, _toiCountdownLabel, _toiDisplayLabel, lblToiInput, _toiTimeField, setToiBtn);

        // ── Module host ─────────────────────────────────────────────────────────
        _moduleHost = new FrameView("Module")
        {
            X = 0, Y = 1, Width = Dim.Fill() - 27, Height = Dim.Fill() - 1
        };

        // ── Status bar ──────────────────────────────────────────────────────────
        _statusBar = new StatusBar(new[]
        {
            new StatusItem(Key.F1, "~F1~ Help", () =>
                MessageBox.Query("Help", "F1 Help  F5 Pause/Resume  F10 Quit\nUse menu to switch modules.", "OK")),
            new StatusItem(Key.F5, "~F5~ Pause", OnTogglePause),
            new StatusItem(Key.F10, "~F10~ Quit", () => Application.RequestStop())
        });

        Add(menuBar, clockFrame, _moduleHost, _statusBar);

        // ── Poll loop ───────────────────────────────────────────────────────────
        Application.MainLoop.AddTimeout(TimeSpan.FromMilliseconds(500), _ =>
        {
            _vm.Tick();
            _currentTimeLabel.Text = _vm.CurrentTime.ToString("HH:mm:ss");
            _toiCountdownLabel.Text = _vm.ToiCountdown;
            _toiDisplayLabel.Text = _vm.ToiDisplayTime;
            return true;
        });

        _vm.OnImpact += FlashScreen;

        // Load the first registered module by default
        if (_registry.Modules.Count > 0)
            ActivateModule(_registry.Modules[0]);
    }

    // ── Module activation ───────────────────────────────────────────────────────

    private MenuBarItem BuildModuleMenu()
    {
        var items = _registry.Modules.Select(m =>
            new MenuItem($"_{m.Title}", m.Category, () => ActivateModule(m))).ToArray();
        return new MenuBarItem("_Modules", items);
    }

    private void ActivateModule(ICalculatorModule module)
    {
        if (_activeModuleView != null)
            _moduleHost.Remove(_activeModuleView);

        _activeModule = module;
        _moduleHost.Title = module.Title;
        _activeModuleView = module.CreateView(_context);
        _activeModuleView.X = 0;
        _activeModuleView.Y = 0;
        _activeModuleView.Width = Dim.Fill();
        _activeModuleView.Height = Dim.Fill();
        _moduleHost.Add(_activeModuleView);
    }

    // ── Actions ──────────────────────────────────────────────────────────────────

    private void OnSetTime()
    {
        var dialog = new TimeSetupDialog();
        Application.Run(dialog);
        _vm.SetClockTime(dialog.SelectedTime);
    }

    private void OnTogglePause()
    {
        if (_vm.IsClockPaused) _vm.ResumeClock();
        else _vm.PauseClock();
    }

    private void OnSetToi()
    {
        var text = _toiTimeField.Text.ToString() ?? "";
        if (!_vm.TrySetToi(text))
            MessageBox.ErrorQuery("Error", "Invalid TOI format. Use HH:mm:ss", "OK");
    }

    // ── Color helpers ─────────────────────────────────────────────────────────

    private void ApplyScheme(View view, ColorScheme scheme)
    {
        view.ColorScheme = scheme;
        foreach (var sub in view.Subviews) ApplyScheme(sub, scheme);
    }

    private void FlashScreen()
    {
        var original = _activeScheme;
        ApplyScheme(this, _flashScheme);
        Application.MainLoop.AddTimeout(TimeSpan.FromMilliseconds(500), _ =>
        {
            ApplyScheme(this, original);
            return false;
        });
    }

    private static ColorScheme MakeScheme(Color normal, Color normalBg, Color focus, Color focusBg, Color hot, Color disabled)
        => new()
        {
            Normal = Terminal.Gui.Attribute.Make(normal, normalBg),
            Focus = Terminal.Gui.Attribute.Make(focus, focusBg),
            HotNormal = Terminal.Gui.Attribute.Make(hot, normalBg),
            HotFocus = Terminal.Gui.Attribute.Make(focus, focusBg),
            Disabled = Terminal.Gui.Attribute.Make(disabled, normalBg)
        };
}
