namespace MyKhronus.WPF.UserControls.Views;

using System.Windows.Controls;

/// <summary>
/// Interaction logic for ActivityUserControl.xaml
/// </summary>
public partial class ActivityUserControl : UserControl
{
    public ActivityUserControl()
    {
        InitializeComponent();
    }

    private void Filter_GotFocus(object sender, System.Windows.RoutedEventArgs e)
    {
        var textBox = (TextBox)e.Source;

        textBox.Text = "";
    }
}
