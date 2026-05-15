namespace MyKhronus.WPF;

using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private void TitleBar_MouseDown(object sender, MouseButtonEventArgs e)
    {
        if (e.ChangedButton != MouseButton.Left) return;

        var source = e.OriginalSource as DependencyObject;
        while (source != null)
        {
            if (source is Button) return;
            source = VisualTreeHelper.GetParent(source);
        }

        if (e.ClickCount == 2)
        {
            Maximize_Click(sender, e);
            return;
        }

        DragMove();
    }

    private void Minimize_Click(object sender, RoutedEventArgs e) => WindowState = WindowState.Minimized;

    private void Maximize_Click(object sender, RoutedEventArgs e)
        => WindowState = WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
    
    private void Close_Click(object sender, RoutedEventArgs e) => Application.Current.Shutdown();

}
