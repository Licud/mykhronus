namespace MyKhronus.WPF.Services;

public interface ILockedSessionTimerService
{
    TimeSpan Duration { get; }

    void StartTimer();

    void StopTimer();

    void ResetDuration();
}