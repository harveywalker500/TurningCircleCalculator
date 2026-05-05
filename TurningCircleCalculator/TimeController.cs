using System.Timers;

namespace TurningCircleCalculator;

/// <summary>
/// The time controller responsible for managing the simulated clock.
/// </summary>
public class TimeController
{
    /// <summary>
    /// The timer that updates the simulated clock every second.
    /// </summary>
    private System.Timers.Timer clockTimer;
    /// <summary>
    /// The current simulated time.
    /// </summary>
    public DateTime Time;

    /// <summary>
    /// Initialises a new instance of the <see cref="TimeController"/> class.
    /// </summary>
    /// <param name="initialTime">The time the <see cref="Time"/> variable is initially set.</param>
    public TimeController(string initialTime)
    {
        InitialiseTime(initialTime);
        StartTimer();
    }

    /// <summary>
    /// Initialises the simulated time using the provided time.
    /// </summary>
    /// <param name="initialTime">The time the <see cref="Time"/> variable is initially set.</param>
    public void InitialiseTime(string? initialTime)
    {
        if (string.IsNullOrEmpty(initialTime))
        {
            Time = DateTime.Now;
            return;
        }

        try
        {
            Time = DateTime.Parse(initialTime);
        }
        catch (FormatException)
        {
            Time = DateTime.Now;
        }
        catch (Exception)
        {
            Time = DateTime.Now;
        }
    }

    /// <summary>
    /// Starts the <see cref="clockTimer"/> to update the simulated time every second.
    /// </summary>
    public void StartTimer()
    {
        clockTimer = new System.Timers.Timer(1000);
        clockTimer.Elapsed += ClockTimerCallback;
        clockTimer.AutoReset = true;
        clockTimer.Enabled = true;
    }

    /// <summary>
    /// Stops or pauses the <see cref="clockTimer"/>.
    /// </summary>
    /// <param name="pause">Flag to determine if the timer is disposed of. Used for pausing the time.</param>
    public void StopTimer(bool pause = false)
    {
        clockTimer.Stop();
        if (pause)
        {
            clockTimer.Dispose();
        }
    }

    /// <summary>
    /// The callback method for the <see cref="clockTimer"/> that increments the simulated time by one second.
    /// </summary>
    /// <param name="o">The source of the event.</param>
    /// <param name="e">The event data.</param>
    private void ClockTimerCallback(object o, ElapsedEventArgs e)
    {
        Time += TimeSpan.FromSeconds(1);

        // For debugging purposes
        // Console.WriteLine($"Time is {Time.TimeOfDay}");
    }
}