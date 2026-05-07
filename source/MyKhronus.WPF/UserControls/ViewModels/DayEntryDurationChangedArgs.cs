namespace MyKhronus.WPF.UserControls.ViewModels;

using MyKhronus.Models.Enums;

public class DayEntryDurationChangedArgs : EventArgs
{
    public TimeSpan DurationChange { get; internal set; }

    public DurationChangeReason DurationChangeReason { get; internal set; }
}
