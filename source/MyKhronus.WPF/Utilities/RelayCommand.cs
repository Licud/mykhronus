namespace MyKhronus.WPF.Utilities;

using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Input;

public class RelayCommand : ICommand, IDisposable
{
    #region Fields

    private Action execute = null;
    private Func<bool> canExecute = null;

    #endregion // Fields

    #region Constructors

    public RelayCommand(Action execute)
        : this(execute, null)
    {
    }
    /// <summary>
    /// Creates a new command.
    /// </summary>
    /// <param name="execute">The execution logic.</param>
    /// <param name="canExecute">The execution status logic.</param>
    public RelayCommand(Action execute, Func<bool> canExecute)
    {
        if (execute == null)
            throw new ArgumentNullException("execute");

        this.execute = execute;
        this.canExecute = canExecute;
    }



    #endregion // Constructors

    #region ICommand Members

    [DebuggerStepThrough]
    public bool CanExecute(object parameter)
    {
        return canExecute == null ? true : canExecute();
    }

    public event EventHandler CanExecuteChanged
    {
        add
        {
            CommandManager.RequerySuggested += value;
            eventHandlers.Add(value);
        }
        remove
        {
            CommandManager.RequerySuggested -= value;
            eventHandlers.Remove(value);
        }
    }

    public void Execute(object parameter)
    {
        this.execute();
    }

    #endregion // ICommand Members

    private readonly List<EventHandler> eventHandlers = new List<EventHandler>();

    public void Dispose()
    {
        this.canExecute = null;
        this.execute = null;
        foreach (EventHandler handler in this.eventHandlers)
        {
            CommandManager.RequerySuggested -= handler;
        }
        this.eventHandlers.Clear();
    }
}
