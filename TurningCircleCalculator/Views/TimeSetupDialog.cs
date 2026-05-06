using Terminal.Gui;

namespace TurningCircleCalculator.Views;

/// <summary>
/// A dialog that prompts the user to set the initial simulated time.
/// </summary>
public class TimeSetupDialog : Dialog
{
    /// <summary>
    /// Gets the time string selected by the user.
    /// </summary>
    public string SelectedTime { get; private set; } = string.Empty;

    private readonly TextField _timeField;

    /// <summary>
    /// Initializes a new instance of the <see cref="TimeSetupDialog"/> class.
    /// </summary>
    public TimeSetupDialog()
    {
            Title = "Initial Time Setup";

            var lbl = new Label("Set the Initial Time (HH:mm:ss):")
            {
                X = Pos.Center(),
                Y = 1
            };

            _timeField = new TextField(DateTime.Now.ToString("HH:mm:ss"))
            {
                X = Pos.Center(),
                Y = 2,
                Width = 12
            };

            var okBtn = new Button("OK")
            {
                IsDefault = true
            };
            okBtn.Clicked += () =>
            {
                SelectedTime = _timeField.Text.ToString();
                Application.RequestStop();
            };

            var cancelBtn = new Button("Cancel");
            cancelBtn.Clicked += () =>
            {
                SelectedTime = DateTime.Now.ToString("HH:mm:ss");
                Application.RequestStop();
            };

        AddButton(okBtn);
        AddButton(cancelBtn);
        Add(lbl, _timeField);

        // Applying a nautical feel to the dialog too
        var nauticalScheme = new ColorScheme()
        {
            Normal = Terminal.Gui.Attribute.Make(Color.White, Color.Blue),
            Focus = Terminal.Gui.Attribute.Make(Color.BrightYellow, Color.Cyan),
            HotNormal = Terminal.Gui.Attribute.Make(Color.BrightCyan, Color.Blue),
            HotFocus = Terminal.Gui.Attribute.Make(Color.BrightYellow, Color.Cyan),
            Disabled = Terminal.Gui.Attribute.Make(Color.Gray, Color.Blue)
        };
        ColorScheme = nauticalScheme;
    }
}
