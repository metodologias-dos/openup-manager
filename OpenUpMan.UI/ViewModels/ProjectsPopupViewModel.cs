﻿using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using OpenUpMan.Domain;
using System.Threading.Tasks;
using System;
using OpenUpMan.Services;
using System.Linq;

namespace OpenUpMan.UI.ViewModels;

public partial class ProjectsPopupViewModel : ViewModelBase
{
    private readonly IProjectUserService? _projectUserService;
    private readonly IProjectService? _projectService;

    // Event to request the window to close
    public event Action? CloseRequested;

    // Event to request a logout (close projects and return to login)
    public event Action? LogoutRequested;

    // Event to request opening the new project dialog
    public event Action? NewProjectDialogRequested;
    
    // Current logged in user
    public User? CurrentUser { get; private set; }

    [ObservableProperty]
    private bool _isLoadingProjects;

    [ObservableProperty]
    private string _loadingMessage = "Cargando proyectos...";

    // Constructor para uso en producción con servicios
    public ProjectsPopupViewModel(User? currentUser, IProjectUserService projectUserService, IProjectService projectService)
    {
        CurrentUser = currentUser;
        _projectUserService = projectUserService;
        _projectService = projectService;
        Projects = new ObservableCollection<ProjectListItemViewModel>();

        NewProjectCommand = new RelayCommand(OnNewProject);
        CloseCommand = new RelayCommand(OnClose);
        OpenProjectCommand = new RelayCommand<ProjectListItemViewModel?>(OnOpenProject);
        DeleteProjectCommand = new AsyncRelayCommand<ProjectListItemViewModel?>(OnDeleteProjectAsync);
        LogoutCommand = new RelayCommand(OnLogout);

        // Cargar proyectos del usuario automáticamente
        _ = LoadUserProjectsAsync();
    }

    // Constructor para tests (sin servicios, con proyectos iniciales)
    public ProjectsPopupViewModel(User? currentUser = null, IEnumerable<ProjectListItemViewModel>? initialProjects = null)
    {
        CurrentUser = currentUser;
        Projects = new ObservableCollection<ProjectListItemViewModel>();

        // If initial projects are provided (e.g. by tests or a service), populate the collection
        if (initialProjects != null)
        {
            foreach (var p in initialProjects)
                Projects.Add(p);
        }

        NewProjectCommand = new RelayCommand(OnNewProject);
        CloseCommand = new RelayCommand(OnClose);
        OpenProjectCommand = new RelayCommand<ProjectListItemViewModel?>(OnOpenProject);
        DeleteProjectCommand = new AsyncRelayCommand<ProjectListItemViewModel?>(OnDeleteProjectAsync);
        LogoutCommand = new RelayCommand(OnLogout);
    }

    private async Task LoadUserProjectsAsync()
    {
        if (CurrentUser == null || _projectUserService == null || _projectService == null)
            return;

        IsLoadingProjects = true;
        LoadingMessage = "Cargando tus proyectos...";

        try
        {
            // 1. Obtener los ProjectUser del usuario actual
            var projectUsers = (await _projectUserService.GetUserProjectsAsync(CurrentUser.Id)).ToList();

            if (!projectUsers.Any())
            {
                LoadingMessage = "No tienes proyectos asignados";
                return;
            }

            // 2. Para cada ProjectUser, obtener el proyecto correspondiente
            var projectTasks = projectUsers.Select(async pu =>
            {
                var projectResult = await _projectService.GetProjectByIdAsync(pu.ProjectId);
                if (projectResult.Success && projectResult.Project != null)
                {
                    return projectResult.Project;
                }
                return null;
            });

            var projects = await Task.WhenAll(projectTasks);

            // 3. Convertir los proyectos a ViewModels y agregarlos a la lista
            Projects.Clear();
            foreach (var project in projects.Where(p => p != null))
            {
                var projectVm = new ProjectListItemViewModel
                {
                    Id = project!.Identifier,
                    Name = project.Name,
                    LastEdited = project.UpdatedAt?.ToString("dd/MM/yyyy HH:mm") 
                                 ?? project.CreatedAt.ToString("dd/MM/yyyy HH:mm")
                };
                Projects.Add(projectVm);
            }

            LoadingMessage = Projects.Count > 0 
                ? $"{Projects.Count} proyecto(s) cargado(s)" 
                : "No tienes proyectos asignados";
        }
        catch (Exception ex)
        {
            LoadingMessage = $"Error al cargar proyectos: {ex.Message}";
            Console.WriteLine($"Error loading projects: {ex}");
        }
        finally
        {
            IsLoadingProjects = false;
        }
    }

    [ObservableProperty]
    private ObservableCollection<ProjectListItemViewModel> _projects = null!;

    public IRelayCommand NewProjectCommand { get; }
    public IRelayCommand CloseCommand { get; }
    public IRelayCommand<ProjectListItemViewModel?> OpenProjectCommand { get; }
    public IAsyncRelayCommand<ProjectListItemViewModel?> DeleteProjectCommand { get; }

    // Command to trigger logout
    public IRelayCommand LogoutCommand { get; }

    private void OnNewProject()
    {
        NewProjectDialogRequested?.Invoke();
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

    private async Task OnDeleteProjectAsync(ProjectListItemViewModel? project)
    {
        if (project == null) return;

        // Si no hay servicio (modo test), solo eliminar de la UI
        if (_projectService == null)
        {
            Projects.Remove(project);
            return;
        }

        try
        {
            // Obtener el proyecto completo por su identificador
            var projectResult = await _projectService.GetProjectByIdentifierAsync(project.Id);
            
            if (!projectResult.Success || projectResult.Project == null)
            {
                Console.WriteLine($"No se pudo encontrar el proyecto {project.Id}");
                return;
            }

            // Eliminar el proyecto de la base de datos
            var deleteResult = await _projectService.DeleteProjectAsync(projectResult.Project.Id);

            if (deleteResult.Success)
            {
                // Eliminar de la UI
                Projects.Remove(project);
                Console.WriteLine($"Proyecto {project.Name} eliminado exitosamente");
            }
            else
            {
                Console.WriteLine($"Error al eliminar proyecto: {deleteResult.Message}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error inesperado al eliminar proyecto: {ex.Message}");
        }
    }

    private void OnLogout()
    {
        LogoutRequested?.Invoke();
    }

    public void AddProject(ProjectDialogResult result)
    {
        var newProject = new ProjectListItemViewModel
        {
            Id = result.Identifier,
            Name = result.Name,
            LastEdited = DateTime.Now.ToString("dd/MM/yyyy HH:mm")
        };
        Projects.Add(newProject);
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
