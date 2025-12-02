using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Threading.Tasks;
using OpenUpMan.Services;
using OpenUpMan.Domain;

namespace OpenUpMan.UI.ViewModels;

public partial class ProjectDialogViewModel : ViewModelBase
{
    private readonly IProjectService _projectService;
    private readonly IProjectUserService _projectUserService;
    private readonly int _currentUserId;

    public event Action? CloseRequested;
    public event Action<ProjectDialogResult>? ProjectCreated;

    [ObservableProperty]
    private string _name = string.Empty;

    [ObservableProperty]
    private string? _code;

    [ObservableProperty]
    private string? _description;

    [ObservableProperty]
    private DateTime? _startDate = DateTime.Now;

    [ObservableProperty]
    private string _errorMessage = string.Empty;
    
    [ObservableProperty]
    private string _dateValidationFeedback = string.Empty;
    
    [ObservableProperty]
    private bool _hasDateValidationFeedback;

    [ObservableProperty]
    private bool _hasError;

    [ObservableProperty]
    private bool _isSaving;


    [ObservableProperty]
    private bool _nameError;

    [ObservableProperty]
    private bool _startDateError;

    public IRelayCommand CreateCommand { get; }
    public IRelayCommand CancelCommand { get; }

    public ProjectDialogViewModel(IProjectService projectService, IProjectUserService projectUserService, int currentUserId)
    {
        _projectService = projectService;
        _projectUserService = projectUserService;
        _currentUserId = currentUserId;
        
        CreateCommand = new AsyncRelayCommand(OnCreateAsync);
        CancelCommand = new RelayCommand(OnCancel);
        
        // Listen to StartDate changes for validation
        PropertyChanged += (s, e) =>
        {
            if (e.PropertyName == nameof(StartDate))
            {
                ValidateStartDate();
            }
        };
    }
    
    private void ValidateStartDate()
    {
        // Clear previous feedback
        HasDateValidationFeedback = false;
        DateValidationFeedback = string.Empty;
        
        // If StartDate is null, set to today and show feedback
        if (StartDate == null)
        {
            StartDate = DateTime.Now;
            DateValidationFeedback = "⚠ Fecha inválida. Se estableció la fecha de hoy.";
            HasDateValidationFeedback = true;
            return;
        }
        
        // If date is in the future more than 1 year, it might be a mistake
        if (StartDate.Value > DateTime.Now.AddYears(1))
        {
            DateValidationFeedback = "⚠ La fecha parece estar muy en el futuro. Verifique que sea correcta.";
            HasDateValidationFeedback = true;
        }
        // If date is more than 10 years in the past, it might be a mistake
        else if (StartDate.Value < DateTime.Now.AddYears(-10))
        {
            DateValidationFeedback = "⚠ La fecha parece estar muy en el pasado. Verifique que sea correcta.";
            HasDateValidationFeedback = true;
        }
    }

    private async Task OnCreateAsync()
    {
        // Reset all field errors
        NameError = false;
        StartDateError = false;
        ClearError();
        
        // Validar campos obligatorios
        bool hasValidationErrors = false;

        if (string.IsNullOrWhiteSpace(Name))
        {
            NameError = true;
            hasValidationErrors = true;
        }

        if (StartDate == null)
        {
            StartDateError = true;
            hasValidationErrors = true;
        }

        if (hasValidationErrors)
        {
            ShowError("Por favor complete todos los campos obligatorios marcados con *");
            return;
        }
        
        // Indicar que se está guardando
        IsSaving = true;

        try
        {
            // Crear el proyecto en la base de datos (el código se autogenerará si no se proporciona)
            var projectResult = await _projectService.CreateProjectAsync(
                name: Name.Trim(),
                createdBy: _currentUserId,
                code: string.IsNullOrWhiteSpace(Code) ? null : Code.Trim(), // null = autogenerar
                description: string.IsNullOrWhiteSpace(Description) ? null : Description.Trim(),
                startDate: StartDate!.Value,
                createPhases: true
            );

            if (!projectResult.Success || projectResult.Project == null)
            {
                ShowError($"Error al crear el proyecto: {projectResult.Message}");
                IsSaving = false;
                return;
            }

            // Agregar el usuario creador al proyecto con rol ADMIN
            var projectUserResult = await _projectUserService.AddUserToProjectAsync(
                projectId: projectResult.Project.Id,
                userId: _currentUserId,
                roleId: RoleIds.Admin
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
                Id = projectResult.Project.Id,
                Code = projectResult.Project.Code ?? "",
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
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime? StartDate { get; set; }
}

