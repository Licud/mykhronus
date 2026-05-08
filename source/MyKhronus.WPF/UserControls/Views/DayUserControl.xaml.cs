namespace MyKhronus.WPF.UserControls.Views;

using System.Windows.Controls;

public partial class DayUserControl : UserControl
{
    public DayUserControl()
    {
        InitializeComponent();
    }

    private void Filter_GotFocus(object sender, System.Windows.RoutedEventArgs e)
    {
        var textBox = (TextBox)e.Source;

        textBox.Text = "";
    }
}
