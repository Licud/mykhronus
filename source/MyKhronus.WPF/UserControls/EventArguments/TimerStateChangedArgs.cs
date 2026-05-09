namespace MyKhronus.WPF.UserControls.EventArguments;

using MyKhronus.Models.Enums;

public class TimerStateChangedArgs : EventArgs
{
    public TimerStateChange TimerStateChange { get; internal set; }
}
