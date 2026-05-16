using System.Timers;

namespace TurningCircleCalculator.Clock;

public class SimulatedClock : IClock, IDisposable
{
    private readonly System.Timers.Timer _timer;
    private DateTime _now;
    private bool _disposed;

    public SimulatedClock(string? initialTime = null)
    {
        _now = ParseOrNow(initialTime);
        _timer = new System.Timers.Timer(1000) { AutoReset = true };
        _timer.Elapsed += OnTick;
        _timer.Start();
    }

    public DateTime Now => _now;
    public bool IsPaused => !_timer.Enabled;

    public void SetTime(string timeString)
    {
        _now = ParseOrNow(timeString);
    }

    public void Pause() => _timer.Stop();

    public void Resume() => _timer.Start();

    public void Dispose()
    {
        if (_disposed) return;
        _timer.Stop();
        _timer.Dispose();
        _disposed = true;
    }

    private void OnTick(object? sender, ElapsedEventArgs e)
    {
        _now = _now.AddSeconds(1);
    }

    internal static DateTime ParseOrNow(string? value)
    {
        if (string.IsNullOrWhiteSpace(value)) return DateTime.Now;
        return DateTime.TryParse(value, out var dt) ? dt : DateTime.Now;
    }
}
