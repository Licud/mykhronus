namespace MyKhronus.WPF.UserControls.ViewModels;

using System.Windows.Input;

using MyKhronus.DataAccess.DayEntries.Models;
using MyKhronus.DataAccess.DayEntries.Services;
using MyKhronus.DataAccess.WorkItems.Models;
using MyKhronus.DataAccess.WorkItems.Services;
using MyKhronus.WPF.Enums;
using MyKhronus.WPF.UserControls.EventArguments;
using MyKhronus.WPF.Utilities;

public class DayEntryViewModel : NotifyPropertyChanged, IDisposable
{
    public event EventHandler Deleted;
    public event EventHandler<DayEntryDurationChangedArgs> DurationChanged;
    public event EventHandler<TimerStateChangedArgs> TimerStateChanged;

    private readonly IDailyEntryService dailyEntryService;
    private readonly IWorkItemService workItemService;

    private DayEntry dayEntry;
    private WorkItem workItem;

    public DayEntryViewModel(
        DayEntry dayEntry, 
        WorkItem workItem, 
        IDailyEntryService dailyEntryService,
        IWorkItemService workItemService,
        ProjectPickerViewModel projectPicker)
    {
        this.dayEntry = dayEntry;
        this.workItem = workItem;
        this.dailyEntryService = dailyEntryService;
        this.workItemService = workItemService;
        Name = workItem.Description;
        
        duration = dayEntry.Duration;
        ProjectPicker = projectPicker;

        ProjectPicker.SelectedProject = workItem.Project;
        ProjectPicker.ProjectChanged += ProjectPicker_ProjectChanged;
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

    private ProjectPickerViewModel projectPicker;

    public ProjectPickerViewModel ProjectPicker
    {
        get { return projectPicker; }
        private set 
        { 
            projectPicker = value;
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

    public void AddDuration(TimeSpan delta)
    {
        Duration = Duration.Add(delta);

        dayEntry = dayEntry with { Duration = Duration };

        DurationChanged?.Invoke(this, new DayEntryDurationChangedArgs
        {
            DurationChange = delta,
            DurationChangeReason = DurationChangeReason.Add
        });
    }

    public ICommand Add => new RelayCommand(ExecuteAdd, () => addMinutes.HasValue);

    private void ExecuteAdd()
    {
        AddDuration(TimeSpan.FromMinutes(addMinutes.Value));
    }

    public ICommand Subtract => new RelayCommand(ExecuteSubtract, () => addMinutes.HasValue);

    private void ExecuteSubtract()
    {
        var delta = TimeSpan.FromMinutes(addMinutes!.Value);
        var result = dayEntry.Duration.Subtract(delta);

        if (result < TimeSpan.Zero)
        {
            return;
        }

        Duration = result;

        dayEntry = dayEntry with { Duration = Duration };

        DurationChanged?.Invoke(this, new DayEntryDurationChangedArgs
        {
            DurationChange = delta,
            DurationChangeReason = DurationChangeReason.Subtract
        });
    }

    public ICommand Reset => new RelayCommand(ExecuteReset);

    private void ExecuteReset()
    {
        var previous = Duration;

        Duration = TimeSpan.Zero;

        dayEntry = dayEntry with { Duration = Duration };

        DurationChanged?.Invoke(this, new DayEntryDurationChangedArgs
        {
            DurationChange = previous,
            DurationChangeReason = DurationChangeReason.Reset
        });
    }

    public ICommand StartTimer => new RelayCommand(ExecuteStartTimer, CanExecuteStartTimer);

    private void ExecuteStartTimer()
    {
        TimerStateChanged?.Invoke(this, new TimerStateChangedArgs
        {
            TimerStateChange = TimerStateChange.Start
        });
    }

    private bool CanExecuteStartTimer() => dayEntry.EntryDate.Date == DateTime.Today;

    public ICommand StopTimer => new RelayCommand(() => 
    {
        TimerStateChanged?.Invoke(this, new TimerStateChangedArgs
        {
            TimerStateChange = TimerStateChange.Stop
        });
    });

    public ICommand Delete => new RelayCommand(async () =>
    {
        await dailyEntryService.Delete(dayEntry.EntryDate, dayEntry.WorkItemId);

        Deleted?.Invoke(this, EventArgs.Empty);
    });

    private void ProjectPicker_ProjectChanged(object sender, EventArgs e)
    {
        var projectPicker = sender as ProjectPickerViewModel;

        if (projectPicker == null || projectPicker.SelectedProject == null)
        {
            workItemService.UnlinkWorkItemToProject(workItem.Id);
        }
        else
        {
            workItemService.LinkWorkItemToProject(workItem.Id, projectPicker.SelectedProject.Id);
        }
    }

    public void Dispose()
    {
        projectPicker.ProjectChanged -= ProjectPicker_ProjectChanged;
    }
}
