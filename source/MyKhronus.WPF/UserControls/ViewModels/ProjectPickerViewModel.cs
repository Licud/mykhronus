namespace MyKhronus.WPF.UserControls.ViewModels;

using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;
using System.Windows.Input;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;

using MyKhronus.DataAccess.Projects.Models;
using MyKhronus.DataAccess.Projects.Services;
using MyKhronus.WPF.Messages;
using MyKhronus.WPF.Utilities;

public class ProjectPickerViewModel : ObservableObject, IRecipient<ProjectAddedMessage>
{
    public event EventHandler ProjectChanged;

    private readonly ObservableCollection<Project> allProjects = [];
    private readonly IProjectService projectService;

    private Project selectedProject;
    private string filterText = "";
    private bool isDropDownOpen;

    public ProjectPickerViewModel(IProjectService projectService, IEnumerable<Project> projects)
    {
        WeakReferenceMessenger.Default.Register(this);

        this.projectService = projectService;

        foreach (var item in projects)
        {
            allProjects.Add(item);
        }

        var cvs = new CollectionViewSource { Source = allProjects };

        FilteredProjects = cvs.View;
        FilteredProjects.Filter = FilterProjects;
    }

    public ICollectionView FilteredProjects { get; }

    public Project SelectedProject
    {
        get => selectedProject;
        set
        {
            selectedProject = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(PlaceholderText));
        }
    }

    public string PlaceholderText => selectedProject?.Name ?? "No project";

    public bool ShowPlaceholder => string.IsNullOrEmpty(filterText);

    public bool HasSelectedProject => SelectedProject != null;

    public string FilterText
    {
        get => filterText;
        set
        {
            filterText = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(CanAddNew));
            OnPropertyChanged(nameof(ShowPlaceholder));
            FilteredProjects.Refresh();
        }
    }

    public bool IsDropDownOpen
    {
        get => isDropDownOpen;
        set
        {
            isDropDownOpen = value;
            OnPropertyChanged();

            if (value)
            {
                FilterText = "";
            }
        }
    }

    public bool CanAddNew()
    {
        return !string.IsNullOrWhiteSpace(filterText) &&
        !allProjects.Any(p => p.Name.Equals(filterText, StringComparison.OrdinalIgnoreCase));
    }

    public ICommand ToggleDropDown => new RelayCommand(() => IsDropDownOpen = !IsDropDownOpen);

    public ICommand ClearProject => new RelayCommand(() =>
    {
        SelectedProject = null;
        IsDropDownOpen = false;
        FilterText = "";
        ProjectChanged?.Invoke(this, EventArgs.Empty);
    });

    public ICommand AddNewProject => new RelayCommand(async () => await ExecuteAddNewProject(), CanAddNew);

    public void SelectProject(Project project)
    {
        SelectedProject = project;
        IsDropDownOpen = false;
        FilterText = "";
        ProjectChanged?.Invoke(this, EventArgs.Empty);
    }

    private async Task ExecuteAddNewProject()
    {
        var project = new NewProject(FilterText);

        var saved = await projectService.Add(project);

        SelectedProject = saved;

        WeakReferenceMessenger.Default.Send(new ProjectAddedMessage(saved));
    }

    private bool FilterProjects(object obj)
    {
        var projectContainsFilter = obj is Project project && project.Name.Contains(filterText, StringComparison.OrdinalIgnoreCase);

        return string.IsNullOrEmpty(filterText) || projectContainsFilter;  
    }

    public void Receive(ProjectAddedMessage message)
    {
        allProjects.Add(message.Project);
    }
}
