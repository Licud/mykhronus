namespace MyKhronus.Commons.Utilities;

using System.ComponentModel;
using System.Runtime.CompilerServices;

public class NotifyPropertyChanged : INotifyPropertyChanged
{

    public event PropertyChangedEventHandler PropertyChanged;

    public virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

}
