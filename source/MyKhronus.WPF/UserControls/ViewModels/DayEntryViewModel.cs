namespace MyKhronus.WPF.UserControls.ViewModels;

using System.Windows.Input;

using MyKhronus.Commons.Utilities;
using MyKhronus.DataAccess.DayEntries.Models;
using MyKhronus.DataAccess.DayEntries.Services;
using MyKhronus.DataAccess.WorkItems.Models;
using MyKhronus.WPF.Utilities;

public class DayEntryViewModel : NotifyPropertyChanged
{
    public event EventHandler Deleted;

    private readonly IDailyEntryService dailyEntryService;

    private DayEntry dayEntry;
    private WorkItem workItem;

    public DayEntryViewModel(DayEntry dayEntry, WorkItem workItem, IDailyEntryService dailyEntryService)
    {
        this.dayEntry = dayEntry;
        this.workItem = workItem;
        this.dailyEntryService = dailyEntryService;
        Name = workItem.Description;
        duration = dayEntry.Duration;
    }

    public Guid WorkItemId => dayEntry.WorkItemId;

    public DayEntry DayEntry => dayEntry;

    public WorkItem WorkItem => workItem;

    public string Name { get; }

    private TimeSpan duration;

    public TimeSpan Duration
    {
        get => duration;
        private set
        {
            duration = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(DurationDisplay));
        }
    }

    public string DurationDisplay => Duration.ToString();

    private bool isTimerRunning;

    public bool IsTimerRunning
    {
        get => isTimerRunning;
        set
        {
            isTimerRunning = value;
            OnPropertyChanged();
        }
    }

    private int? addMinutes = 5;

    public string AddMins
    {
        get => addMinutes.HasValue ? addMinutes.Value.ToString() : "";
        set
        {
            if (string.IsNullOrWhiteSpace(value))
                addMinutes = null;
            else if (int.TryParse(value, out int parsed))
                addMinutes = parsed;

            OnPropertyChanged();
        }
    }

    public ICommand Add => new RelayCommand(async () => await ExecuteAdd(), () => addMinutes.HasValue);

    private async Task ExecuteAdd()
    {
        var delta = TimeSpan.FromMinutes(addMinutes!.Value);

        Duration = Duration.Add(delta);

        dayEntry = dayEntry with { Duration = Duration };

        await dailyEntryService.Update(dayEntry);
    }

    public ICommand Subtract => new RelayCommand(async () => await ExecuteSubtract(), () => addMinutes.HasValue);

    private async Task ExecuteSubtract()
    {
        var delta = TimeSpan.FromMinutes(addMinutes!.Value);
        var result = dayEntry.Duration.Subtract(delta);

        if (result >= TimeSpan.Zero)
        {
            Duration = result;

            dayEntry = dayEntry with { Duration = Duration };

            await dailyEntryService.Update(dayEntry);
        }
    }

    public ICommand Reset => new RelayCommand(async () => await ExecuteResetAsync());

    private async Task ExecuteResetAsync()
    {
        Duration = TimeSpan.Zero;

        dayEntry = dayEntry with { Duration = Duration };

        await dailyEntryService.Update(dayEntry);
    }

    public ICommand StartTimer => new RelayCommand(() => { });

    public ICommand PauseTimer => new RelayCommand(() => { });

    public ICommand Delete => new RelayCommand(async () =>
    {
        await dailyEntryService.Delete(dayEntry.EntryDate, dayEntry.WorkItemId);

        Deleted?.Invoke(this, EventArgs.Empty);
    });
}
