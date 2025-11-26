using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;

namespace OpenUpMan.UI.ViewModels;

public partial class ProjectDialogViewModel : ViewModelBase
{
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

    public IRelayCommand CreateCommand { get; }
    public IRelayCommand CancelCommand { get; }

    public ProjectDialogViewModel()
    {
        CreateCommand = new RelayCommand(OnCreate);
        CancelCommand = new RelayCommand(OnCancel);
    }

    private void OnCreate()
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

        // Crear resultado
        var result = new ProjectDialogResult
        {
            Identifier = Identifier.Trim(),
            Name = Name.Trim(),
            Description = string.IsNullOrWhiteSpace(Description) ? null : Description.Trim(),
            StartDate = StartDate.Value.DateTime
        };

        // Notificar que se creó el proyecto
        ProjectCreated?.Invoke(result);

        // Cerrar el diálogo
        CloseRequested?.Invoke();
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

