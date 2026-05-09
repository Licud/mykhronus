namespace MyKhronus.WPF.UserControls.Views;

using System.Windows;
using System.Windows.Controls;

using MyKhronus.WPF.UserControls.ViewModels;

public partial class DayUserControl : UserControl
{
    public DayUserControl()
    {
        InitializeComponent();

        Loaded += DayUserControl_Loaded;
    }

    private void DayUserControl_Loaded(object sender, RoutedEventArgs e)
    {
        var viewModel = DataContext as DayUserControlViewModel;

        if (viewModel?.Loaded.CanExecute(null) == true)
        {
            viewModel.Loaded.Execute(null);
        }
    }

    private void Filter_GotFocus(object sender, RoutedEventArgs e)
    {
        var textBox = (TextBox)e.Source;

        textBox.Text = "";
    }
}
