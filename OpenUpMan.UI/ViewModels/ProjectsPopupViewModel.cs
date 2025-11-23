using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using OpenUpMan.Domain;
using System.Threading.Tasks;
using System.Threading;
using System;

namespace OpenUpMan.UI.ViewModels;

public partial class ProjectsPopupViewModel : ViewModelBase
{
    // Event to request the window to close
    public event Action? CloseRequested;

    // Event to request a logout (close projects and return to login)
    public event Action? LogoutRequested;

    public ProjectsPopupViewModel()
    {
        Projects = new ObservableCollection<ProjectListItemViewModel>();
        // Sample data placeholder - in future, load from a projects service
        Projects.Add(new ProjectListItemViewModel { Id = "PRJ-001", Name = "Sistema de Gestión Empresarial", LastEdited = "2025-11-23" });
        Projects.Add(new ProjectListItemViewModel { Id = "PRJ-002", Name = "Portal Web Corporativo", LastEdited = "2025-11-22" });
        Projects.Add(new ProjectListItemViewModel { Id = "PRJ-003", Name = "App Mobile de Ventas", LastEdited = "2025-11-20" });
        Projects.Add(new ProjectListItemViewModel { Id = "PRJ-004", Name = "Dashboard Analítico", LastEdited = "2025-11-18" });
        Projects.Add(new ProjectListItemViewModel { Id = "PRJ-005", Name = "Sistema de Inventario", LastEdited = "2025-11-15" });

        NewProjectCommand = new RelayCommand(OnNewProject);
        CloseCommand = new RelayCommand(OnClose);
        OpenProjectCommand = new RelayCommand<ProjectListItemViewModel?>(OnOpenProject);
        DeleteProjectCommand = new RelayCommand<ProjectListItemViewModel?>(OnDeleteProject);

        // logout command
        LogoutCommand = new RelayCommand(OnLogout);
    }

    [ObservableProperty]
    private ObservableCollection<ProjectListItemViewModel> _projects = null!;

    public IRelayCommand NewProjectCommand { get; }
    public IRelayCommand CloseCommand { get; }
    public IRelayCommand<ProjectListItemViewModel?> OpenProjectCommand { get; }
    public IRelayCommand<ProjectListItemViewModel?> DeleteProjectCommand { get; }

    // Command to trigger logout
    public IRelayCommand LogoutCommand { get; }

    private void OnNewProject()
    {
        // TODO: open a new project creation window
    }

    private void OnClose()
    {
        CloseRequested?.Invoke();
    }

    private void OnOpenProject(ProjectListItemViewModel? project)
    {
        if (project == null) return;
        // TODO: open the project in a new Window
    }

    private void OnDeleteProject(ProjectListItemViewModel? project)
    {
        if (project == null) return;
        Projects.Remove(project);
    }

    private void OnLogout()
    {
        LogoutRequested?.Invoke();
    }
}

public partial class ProjectListItemViewModel : ObservableObject
{
    [ObservableProperty]
    private string _id = string.Empty;

    [ObservableProperty]
    private string _name = string.Empty;

    [ObservableProperty]
    private string _lastEdited = string.Empty;
}
