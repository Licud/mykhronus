namespace MyKhronus.WPF.UserControls.EventArguments;

using MyKhronus.WPF.Enums;

public class TimerStateChangedArgs : EventArgs
{
    public TimerStateChange TimerStateChange { get; internal set; }
}
