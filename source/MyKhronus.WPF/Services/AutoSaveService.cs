namespace MyKhronus.WPF.Services;

using System.Timers;

public class AutoSaveService : IAutoSaveService
{
    private const int numberOfSecondsInaMinute = 60;
    private const int intervalInMilliseconds = 1000;

    private IActivityRecordsService activityRecordsService;

    private readonly Timer sessionTimer = new Timer();

    public AutoSaveService(IActivityRecordsService activityRecordsService)
    {
        this.activityRecordsService = activityRecordsService;

        sessionTimer.Elapsed += SessionTimer_Elapsed;

        SetUpInterval();
    }

    public void StartTimer() => sessionTimer.Start();

    public void StopTimer() => sessionTimer.Stop();

    private void SetUpInterval() => sessionTimer.Interval = intervalInMilliseconds * numberOfSecondsInaMinute * 1;

    private void SessionTimer_Elapsed(object sender, ElapsedEventArgs e)
        => activityRecordsService.Save();

}
