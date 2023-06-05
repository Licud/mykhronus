namespace MyKhronus.WPF.Windows
{
    using System.Windows;

    /// <summary>
    /// Interaction logic for ConfirmWindow.xaml
    /// </summary>
    public partial class ConfirmWindow : Window
    {
        public ConfirmWindow()
        {
            InitializeComponent();
        }

        public ConfirmWindow(string message, string title) 
            :this()
        {
            Title = title;

            ConfirmMessage.Text = message;
        }

        private void OK_Click(object sender, RoutedEventArgs e)
            => DialogResult = true;

        private void Cancel_Click(object sender, RoutedEventArgs e)
            => DialogResult = false;
    }
}
