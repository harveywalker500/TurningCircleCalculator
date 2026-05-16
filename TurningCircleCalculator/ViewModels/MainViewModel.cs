using TurningCircleCalculator.Clock;

namespace TurningCircleCalculator.ViewModels;

public class MainViewModel : ViewModelBase
{
    private readonly IClock _clock;
    private DateTime? _toiTime;
    private bool _isToiActive;
    private string _toiCountdown = "TOI: Not Set";
    private string _toiDisplayTime = "TOI @ --:--:--";

    public MainViewModel(IClock clock)
    {
        _clock = clock;
    }

    public IClock Clock => _clock;

    public DateTime CurrentTime => _clock.Now;
    public bool IsClockPaused => _clock.IsPaused;

    public DateTime? ToiTime
    {
        get => _toiTime;
        set { SetField(ref _toiTime, value); Tick(); }
    }

    public bool IsToiActive
    {
        get => _isToiActive;
        private set => SetField(ref _isToiActive, value);
    }

    public string ToiCountdown
    {
        get => _toiCountdown;
        private set => SetField(ref _toiCountdown, value);
    }

    public string ToiDisplayTime
    {
        get => _toiDisplayTime;
        private set => SetField(ref _toiDisplayTime, value);
    }

    public event Action? OnImpact;

    /// <summary>Called by the UI poll loop (every 500 ms) to refresh time-dependent state.</summary>
    public void Tick()
    {
        OnPropertyChanged(nameof(CurrentTime));
        OnPropertyChanged(nameof(IsClockPaused));
        UpdateToiCountdown();
    }

    public void PauseClock()
    {
        _clock.Pause();
        OnPropertyChanged(nameof(IsClockPaused));
    }

    public void ResumeClock()
    {
        _clock.Resume();
        OnPropertyChanged(nameof(IsClockPaused));
    }

    public void SetClockTime(string timeString)
    {
        _clock.SetTime(timeString);
        Tick();
    }

    public bool TrySetToi(string timeString)
    {
        if (!DateTime.TryParse(timeString, out var toi)) return false;
        var now = _clock.Now;
        var target = new DateTime(now.Year, now.Month, now.Day, toi.Hour, toi.Minute, toi.Second);
        if (target < now) target = target.AddDays(1);
        ToiTime = target;
        return true;
    }

    private void UpdateToiCountdown()
    {
        if (_toiTime == null)
        {
            ToiCountdown = "TOI: Not Set";
            ToiDisplayTime = "TOI @ --:--:--";
            IsToiActive = false;
            return;
        }

        ToiDisplayTime = $"TOI @ {_toiTime.Value:HH:mm:ss}";
        var diff = _toiTime.Value - _clock.Now;

        if (diff.TotalSeconds <= 0)
        {
            ToiCountdown = "IMPACT!";
            if (!IsToiActive)
            {
                IsToiActive = true;
                OnImpact?.Invoke();
            }
        }
        else
        {
            ToiCountdown = $"TOI: {diff:hh\\:mm\\:ss}";
            IsToiActive = false;
        }
    }
}
