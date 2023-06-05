namespace MyKhronus.WPF.Services;

using System.Timers;

public class LockedSessionTimerService : ILockedSessionTimerService
{
    private const int intervalInMilliseconds = 1000;

    private readonly Timer sessionTimer = new Timer(intervalInMilliseconds);

    public LockedSessionTimerService()
    {
        sessionTimer.Elapsed += SessionTimer_Elapsed;
    }

    public TimeSpan Duration { get; private set; } = new TimeSpan(0, 0, 0);

    public void StartTimer() => sessionTimer.Start();

    public void StopTimer() => sessionTimer.Stop();

    private void SessionTimer_Elapsed(object sender, ElapsedEventArgs e)
        => Duration = Duration.Add(new TimeSpan(0, 0, 1));

    public void ResetDuration() => Duration = new TimeSpan(0, 0, 0);

}
