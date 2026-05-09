namespace MyKhronus.WPF.UserControls.Views
{
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Media;

    /// <summary>
    /// Interaction logic for ReportsUserControl.xaml
    /// </summary>
    public partial class ReportsUserControl : UserControl
    {
        public ReportsUserControl()
        {
            InitializeComponent();
        }

        private void RecordsScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.Handled || sender is not ScrollViewer scrollViewer)
            {
                return;
            }

            e.Handled = true;

            var bubbled = new MouseWheelEventArgs(e.MouseDevice, e.Timestamp, e.Delta)
            {
                RoutedEvent = UIElement.MouseWheelEvent,
                Source = sender,
            };

            var parent = VisualTreeHelper.GetParent(scrollViewer) as UIElement;

            parent?.RaiseEvent(bubbled);
        }
    }
}
