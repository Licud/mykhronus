namespace MyKhronus.Models.EventArgs;

using MyKhronus.Models.Enums;

public class ActivityRecordDurationChangedArgs : System.EventArgs
{
    public TimeSpan DurationChange { get; internal set; }

    public DurationChangeReason DurationChangeReason { get; internal set; }

}
