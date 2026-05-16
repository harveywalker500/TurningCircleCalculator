using TurningCircleCalculator.Clock;

namespace TurningCircleCalculator.Tests.Clock;

public class FakeClock : IClock
{
    public DateTime Now { get; private set; }
    public bool IsPaused { get; private set; }

    public FakeClock(DateTime? initial = null)
    {
        Now = initial ?? new DateTime(2024, 1, 1, 12, 0, 0);
    }

    public void Advance(TimeSpan by) => Now = Now.Add(by);
    public void AdvanceSeconds(int seconds) => Advance(TimeSpan.FromSeconds(seconds));

    public void SetTime(string timeString)
    {
        if (DateTime.TryParse(timeString, out var dt)) Now = dt;
    }

    public void Pause() => IsPaused = true;
    public void Resume() => IsPaused = false;
}
