namespace MyKhronus.WPF.UserControls.ViewModels;

using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;
using System.Windows.Input;

using MyKhronus.DataAccess.Projects.Models;
using MyKhronus.WPF.Utilities;

public class ProjectPickerViewModel : NotifyPropertyChanged
{
    private readonly ObservableCollection<Project> allProjects = [];

    private Project selectedProject;
    private string filterText = "";
    private bool isDropDownOpen;

    public ProjectPickerViewModel()
    {
        var cvs = new CollectionViewSource { Source = allProjects };

        FilteredProjects = cvs.View;
        FilteredProjects.Filter = FilterProjects;
    }

    public event EventHandler ProjectChanged;

    public ICollectionView FilteredProjects { get; }

    public Project SelectedProject
    {
        get => selectedProject;
        private set
        {
            selectedProject = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(PlaceholderText));
        }
    }

    public string PlaceholderText => selectedProject?.Name ?? "No project";

    public bool ShowPlaceholder => string.IsNullOrEmpty(filterText);

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
                FilterText = "";
        }
    }

    public bool CanAddNew =>
        !string.IsNullOrWhiteSpace(filterText) &&
        !allProjects.Any(p => p.Name.Equals(filterText, StringComparison.OrdinalIgnoreCase));

    public ICommand ToggleDropDown => new RelayCommand(() => IsDropDownOpen = !IsDropDownOpen);

    public ICommand ClearProject => new RelayCommand(() =>
    {
        SelectedProject = null;
        IsDropDownOpen = false;
        FilterText = "";
        ProjectChanged?.Invoke(this, EventArgs.Empty);
    });

    public ICommand AddNewProject => new RelayCommand(ExecuteAddNewProject, () => CanAddNew);

    public void SelectProject(Project project)
    {
        SelectedProject = project;
        IsDropDownOpen = false;
        FilterText = "";
        ProjectChanged?.Invoke(this, EventArgs.Empty);
    }

    public void LoadProjects(IEnumerable<Project> projects, int? selectedProjectId = null)
    {
        allProjects.Clear();

        foreach (var project in projects.OrderBy(p => p.Name))
            allProjects.Add(project);

        if (selectedProjectId.HasValue)
            SelectedProject = allProjects.FirstOrDefault(p => p.Id == selectedProjectId.Value);
    }

    public void AddProject(Project project)
    {
        var insertIndex = allProjects
            .TakeWhile(p => string.Compare(p.Name, project.Name, StringComparison.OrdinalIgnoreCase) < 0)
            .Count();

        allProjects.Insert(insertIndex, project);

        SelectedProject = project;
        IsDropDownOpen = false;
        FilterText = "";
        ProjectChanged?.Invoke(this, EventArgs.Empty);
    }

    private void ExecuteAddNewProject()
    {
        // Stub — caller creates the project then calls AddProject(result)
    }

    private bool FilterProjects(object obj) =>
        string.IsNullOrEmpty(filterText) ||
        obj is Project project && project.Name.Contains(filterText, StringComparison.OrdinalIgnoreCase);
}
