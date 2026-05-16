namespace TurningCircleCalculator.Clock;

public interface IClock
{
    DateTime Now { get; }
    bool IsPaused { get; }
    void SetTime(string timeString);
    void Pause();
    void Resume();
}
