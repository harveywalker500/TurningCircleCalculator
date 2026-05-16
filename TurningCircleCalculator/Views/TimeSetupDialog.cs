using Terminal.Gui;
using TurningCircleCalculator.ViewModels;

namespace TurningCircleCalculator.Views;

public class TimeSetupDialog : Dialog
{
    private readonly TimeSetupViewModel _vm;
    private readonly TextField _timeField;
    private readonly Label _errorLabel;

    public string SelectedTime => _vm.SelectedTime ?? DateTime.Now.ToString("HH:mm:ss");

    public TimeSetupDialog()
    {
        _vm = new TimeSetupViewModel(DateTime.Now.ToString("HH:mm:ss"));
        Title = "Initial Time Setup";

        var lbl = new Label("Set the Initial Time (HH:mm:ss):") { X = Pos.Center(), Y = 1 };
        _timeField = new TextField(_vm.TimeInput) { X = Pos.Center(), Y = 2, Width = 12 };
        _timeField.TextChanged += _ => _vm.TimeInput = _timeField.Text.ToString() ?? string.Empty;

        _errorLabel = new Label("") { X = Pos.Center(), Y = 3, Width = Dim.Fill() - 2 };

        var okBtn = new Button("OK") { IsDefault = true };
        okBtn.Clicked += () =>
        {
            if (_vm.Confirm()) Application.RequestStop();
            else _errorLabel.Text = _vm.ErrorMessage ?? "";
        };

        var cancelBtn = new Button("Cancel");
        cancelBtn.Clicked += () =>
        {
            _vm.TimeInput = DateTime.Now.ToString("HH:mm:ss");
            _vm.Confirm();
            Application.RequestStop();
        };

        AddButton(okBtn);
        AddButton(cancelBtn);
        Add(lbl, _timeField, _errorLabel);

        ColorScheme = NauticalScheme();
    }

    private static ColorScheme NauticalScheme() => new()
    {
        Normal = Terminal.Gui.Attribute.Make(Color.White, Color.Blue),
        Focus = Terminal.Gui.Attribute.Make(Color.BrightYellow, Color.Cyan),
        HotNormal = Terminal.Gui.Attribute.Make(Color.BrightCyan, Color.Blue),
        HotFocus = Terminal.Gui.Attribute.Make(Color.BrightYellow, Color.Cyan),
        Disabled = Terminal.Gui.Attribute.Make(Color.Gray, Color.Blue)
    };
}
