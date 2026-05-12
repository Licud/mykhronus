namespace MyKhronus.WPF.UserControls.Views;

using System.Windows;
using System.Windows.Controls;

using MyKhronus.DataAccess.Projects.Models;
using MyKhronus.WPF.UserControls.ViewModels;

public partial class ProjectPickerUserControl : UserControl
{
    public ProjectPickerUserControl()
    {
        InitializeComponent();
    }

    private void Popup_Opened(object sender, EventArgs e) => FilterBox.Focus();

    private void ProjectItem_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button button &&
            button.Tag is Project project &&
            DataContext is ProjectPickerViewModel vm)
        {
            vm.SelectProject(project);
        }
    }
}
