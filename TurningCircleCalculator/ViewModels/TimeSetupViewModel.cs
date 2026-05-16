using TurningCircleCalculator.Clock;

namespace TurningCircleCalculator.ViewModels;

public class TimeSetupViewModel : ViewModelBase
{
    private string _timeInput;
    private string? _errorMessage;

    public TimeSetupViewModel(string defaultTime)
    {
        _timeInput = defaultTime;
    }

    public string TimeInput
    {
        get => _timeInput;
        set
        {
            SetField(ref _timeInput, value);
            ErrorMessage = null;
        }
    }

    public string? ErrorMessage
    {
        get => _errorMessage;
        private set => SetField(ref _errorMessage, value);
    }

    public string? SelectedTime { get; private set; }

    public bool Confirm()
    {
        if (DateTime.TryParse(TimeInput, out _))
        {
            SelectedTime = TimeInput;
            return true;
        }
        ErrorMessage = "Invalid time format. Use HH:mm:ss";
        return false;
    }
}
