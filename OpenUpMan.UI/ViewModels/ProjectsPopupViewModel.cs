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
    
    // Display username
    public string UserDisplayName => CurrentUser != null ? $"Usuario: {CurrentUser.Username}" : string.Empty;
    public bool HasUser => CurrentUser != null;

    [ObservableProperty]
    private bool _isLoadingProjects;

    [ObservableProperty]
    private string _loadingMessage = "Cargando proyectos...";

    // Sorting state
    private enum SortState { None, Ascending, Descending }
    private SortState _nameSortState = SortState.None;
    private SortState _dateSortState = SortState.None;
    private List<ProjectListItemViewModel> _originalProjectsList = new();

    [ObservableProperty]
    private string _nameSortIndicator = "";

    [ObservableProperty]
    private string _dateSortIndicator = "";

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
        SortByNameCommand = new RelayCommand(SortByName);
        SortByDateCommand = new RelayCommand(SortByDate);

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
        SortByNameCommand = new RelayCommand(SortByName);
        SortByDateCommand = new RelayCommand(SortByDate);
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
                // Convertir UTC a hora local para mostrar
                var lastEditedDate = project!.UpdatedAt ?? project.CreatedAt;
                var lastEditedLocal = DateTime.SpecifyKind(lastEditedDate, DateTimeKind.Utc).ToLocalTime();
                
                var projectVm = new ProjectListItemViewModel
                {
                    Id = project.Identifier,
                    Name = project.Name,
                    LastEdited = lastEditedLocal.ToString("dd/MM/yyyy HH:mm")
                };
                Projects.Add(projectVm);
            }

            LoadingMessage = Projects.Count > 0 
                ? $"{Projects.Count} proyecto(s) cargado(s)" 
                : "No tienes proyectos asignados";
            
            // Inicializar la lista original con los proyectos cargados
            _originalProjectsList = Projects.ToList();
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
    public IRelayCommand LogoutCommand { get; }
    public IRelayCommand SortByNameCommand { get; }
    public IRelayCommand SortByDateCommand { get; }

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
            LastEdited = DateTime.Now.ToString("dd/MM/yyyy HH:mm") // Ya es hora local
        };
        
        // Agregar a la lista original primero
        _originalProjectsList.Add(newProject);
        
        // Si hay sorting activo, aplicarlo; si no, simplemente agregar al final
        if (_nameSortState != SortState.None || _dateSortState != SortState.None)
        {
            // Re-aplicar el sorting actual
            ApplySorting();
        }
        else
        {
            // No hay sorting activo, agregar al final
            Projects.Add(newProject);
        }
    }

    private void SortByName()
    {
        // Guardar la lista original si es la primera vez
        if (_originalProjectsList.Count == 0)
        {
            _originalProjectsList = Projects.ToList();
        }

        // Ciclar entre: Ascending → Descending → None → Ascending
        _nameSortState = _nameSortState switch
        {
            SortState.None => SortState.Ascending,      // A-Z
            SortState.Ascending => SortState.Descending, // Z-A
            SortState.Descending => SortState.None,      // Sin orden
            _ => SortState.Ascending
        };

        // Reset date sort
        _dateSortState = SortState.None;
        DateSortIndicator = "";

        // Update sort indicator
        NameSortIndicator = _nameSortState switch
        {
            SortState.Ascending => " ▲",    // A-Z
            SortState.Descending => " ▼",   // Z-A
            _ => ""                          // Sin orden
        };

        ApplySorting();
    }

    private void SortByDate()
    {
        // Guardar la lista original si es la primera vez
        if (_originalProjectsList.Count == 0)
        {
            _originalProjectsList = Projects.ToList();
        }

        // Ciclar entre: Descending (Recent) → Ascending (Oldest) → None → Descending
        _dateSortState = _dateSortState switch
        {
            SortState.None => SortState.Descending,      // Más reciente primero
            SortState.Descending => SortState.Ascending, // Más antiguo primero
            SortState.Ascending => SortState.None,       // Sin orden
            _ => SortState.Descending
        };

        // Reset name sort
        _nameSortState = SortState.None;
        NameSortIndicator = "";

        // Update sort indicator
        DateSortIndicator = _dateSortState switch
        {
            SortState.Descending => " ▲",   // Más reciente primero (up = recent/nuevo)
            SortState.Ascending => " ▼",    // Más antiguo primero (down = old/viejo)
            _ => ""                          // Sin orden
        };

        ApplySorting();
    }

    private void ApplySorting()
    {
        List<ProjectListItemViewModel> sortedList;

        if (_nameSortState == SortState.Ascending)
        {
            sortedList = _originalProjectsList.OrderBy(p => p.Name).ToList();
        }
        else if (_nameSortState == SortState.Descending)
        {
            sortedList = _originalProjectsList.OrderByDescending(p => p.Name).ToList();
        }
        else if (_dateSortState == SortState.Descending)
        {
            // Más reciente primero
            sortedList = _originalProjectsList.OrderByDescending(p => ParseDate(p.LastEdited)).ToList();
        }
        else if (_dateSortState == SortState.Ascending)
        {
            // Más antiguo primero
            sortedList = _originalProjectsList.OrderBy(p => ParseDate(p.LastEdited)).ToList();
        }
        else
        {
            // Restaurar orden original
            sortedList = _originalProjectsList.ToList();
        }

        // Actualizar la colección
        Projects.Clear();
        foreach (var project in sortedList)
        {
            Projects.Add(project);
        }
    }

    private DateTime ParseDate(string dateString)
    {
        // Parsear la fecha en formato "dd/MM/yyyy HH:mm"
        if (DateTime.TryParseExact(dateString, "dd/MM/yyyy HH:mm", 
            System.Globalization.CultureInfo.InvariantCulture, 
            System.Globalization.DateTimeStyles.None, 
            out var date))
        {
            return date;
        }
        return DateTime.MinValue;
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
