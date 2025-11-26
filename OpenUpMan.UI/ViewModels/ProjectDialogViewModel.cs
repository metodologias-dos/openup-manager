using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Threading.Tasks;
using OpenUpMan.Services;

namespace OpenUpMan.UI.ViewModels;

public partial class ProjectDialogViewModel : ViewModelBase
{
    private readonly IProjectService _projectService;
    private readonly IProjectUserService _projectUserService;
    private readonly Guid _currentUserId;

    public event Action? CloseRequested;
    public event Action<ProjectDialogResult>? ProjectCreated;

    [ObservableProperty]
    private string _identifier = string.Empty;

    [ObservableProperty]
    private string _name = string.Empty;

    [ObservableProperty]
    private string? _description;

    [ObservableProperty]
    private DateTimeOffset? _startDate;

    [ObservableProperty]
    private string _errorMessage = string.Empty;

    [ObservableProperty]
    private bool _hasError;

    [ObservableProperty]
    private bool _isSaving;

    public IRelayCommand CreateCommand { get; }
    public IRelayCommand CancelCommand { get; }

    public ProjectDialogViewModel(IProjectService projectService, IProjectUserService projectUserService, Guid currentUserId)
    {
        _projectService = projectService;
        _projectUserService = projectUserService;
        _currentUserId = currentUserId;
        
        CreateCommand = new AsyncRelayCommand(OnCreateAsync);
        CancelCommand = new RelayCommand(OnCancel);
    }

    private async Task OnCreateAsync()
    {
        // Validar campos obligatorios
        if (string.IsNullOrWhiteSpace(Identifier))
        {
            ShowError("El identificador es obligatorio");
            return;
        }

        if (string.IsNullOrWhiteSpace(Name))
        {
            ShowError("El nombre del proyecto es obligatorio");
            return;
        }

        if (StartDate == null)
        {
            ShowError("La fecha de inicio es obligatoria");
            return;
        }

        // Limpiar error si todo está bien
        ClearError();
        
        // Indicar que se está guardando
        IsSaving = true;

        try
        {
            // Crear el proyecto en la base de datos
            var projectResult = await _projectService.CreateProjectAsync(
                identifier: Identifier.Trim(),
                name: Name.Trim(),
                startDate: StartDate.Value.DateTime,
                ownerId: _currentUserId,
                description: string.IsNullOrWhiteSpace(Description) ? null : Description.Trim()
            );

            if (!projectResult.Success || projectResult.Project == null)
            {
                ShowError($"Error al crear el proyecto: {projectResult.Message}");
                IsSaving = false;
                return;
            }

            // Agregar el usuario creador al proyecto con permisos de OWNER y rol ADMIN
            var projectUserResult = await _projectUserService.AddUserToProjectAsync(
                projectId: projectResult.Project.Id,
                userId: _currentUserId,
                permissions: OpenUpMan.Domain.ProjectUserPermission.OWNER,
                role: OpenUpMan.Domain.ProjectUserRole.ADMIN
            );

            if (!projectUserResult.Success)
            {
                ShowError($"Proyecto creado pero error al asignar permisos: {projectUserResult.Message}");
                IsSaving = false;
                return;
            }

            // Crear resultado para la UI
            var result = new ProjectDialogResult
            {
                Identifier = projectResult.Project.Identifier,
                Name = projectResult.Project.Name,
                Description = projectResult.Project.Description,
                StartDate = projectResult.Project.StartDate
            };

            // Notificar que se creó el proyecto
            ProjectCreated?.Invoke(result);

            // Cerrar el diálogo
            CloseRequested?.Invoke();
        }
        catch (Exception ex)
        {
            ShowError($"Error inesperado: {ex.Message}");
        }
        finally
        {
            IsSaving = false;
        }
    }

    private void OnCancel()
    {
        CloseRequested?.Invoke();
    }

    private void ShowError(string message)
    {
        ErrorMessage = message;
        HasError = true;
    }

    private void ClearError()
    {
        ErrorMessage = string.Empty;
        HasError = false;
    }
}

/// <summary>
/// Resultado del diálogo de creación de proyecto
/// </summary>
public class ProjectDialogResult
{
    public string Identifier { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime StartDate { get; set; }
}

