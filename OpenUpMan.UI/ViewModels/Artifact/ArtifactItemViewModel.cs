using System;
using CommunityToolkit.Mvvm.ComponentModel;

namespace OpenUpMan.UI.ViewModels;

/// <summary>
/// ViewModel que representa un artefacto en la interfaz de usuario.
/// </summary>
public partial class ArtifactItemViewModel : ViewModelBase
{
    [ObservableProperty]
    private int _id;

    [ObservableProperty]
    private int _projectId;

    [ObservableProperty]
    private int _phaseId;

    [ObservableProperty]
    private string _name = string.Empty;

    [ObservableProperty]
    private string? _artifactType;

    [ObservableProperty]
    private bool _mandatory;

    [ObservableProperty]
    private string? _description;

    [ObservableProperty]
    private string _currentState = "PENDING";

    [ObservableProperty]
    private int _currentVersion;

    [ObservableProperty]
    private DateTime? _lastModified;

    [ObservableProperty]
    private DateTime? _lastUpdated;

    [ObservableProperty]
    private string? _lastUpdatedBy;

    public string MandatoryIndicator => Mandatory ? "⭐ Obligatorio" : "Opcional";
    
    public string MandatoryDisplay => Mandatory ? "Sí" : "No";
    
    public string StateDisplay => CurrentState switch
    {
        "PENDING" => "Pendiente",
        "IN_PROGRESS" => "En Progreso",
        "COMPLETED" => "Completado",
        "APPROVED" => "Aprobado",
        _ => CurrentState
    };

    public string VersionDisplay => CurrentVersion > 0 ? $"v{CurrentVersion}" : "Sin versión";
}

