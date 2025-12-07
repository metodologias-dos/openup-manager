using System;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace OpenUpMan.UI.ViewModels;

/// <summary>
/// ViewModel para la ventana de administración de artefactos.
/// Permite gestionar artefactos por fase: crear, editar, eliminar y configurar si son obligatorios.
/// </summary>
public partial class ArtifactAdministrationViewModel : ViewModelBase
{
    #region Properties

    [ObservableProperty]
    private int _projectId;

    [ObservableProperty]
    private string _projectName = string.Empty;

    [ObservableProperty]
    private int _selectedPhaseId;

    [ObservableProperty]
    private string _selectedPhaseName = "Inicio (Inception)";

    [ObservableProperty]
    private ObservableCollection<ArtifactItemViewModel> _artifacts = new();

    [ObservableProperty]
    private ArtifactItemViewModel? _selectedArtifact;

    [ObservableProperty]
    private bool _isEditMode;

    [ObservableProperty]
    private string _editName = string.Empty;

    [ObservableProperty]
    private string? _editArtifactType;

    [ObservableProperty]
    private bool _editMandatory;

    [ObservableProperty]
    private string? _editDescription;

    #endregion

    #region Commands

    public IRelayCommand<string> SelectPhaseCommand { get; }
    public IRelayCommand CreateArtifactCommand { get; }
    public IRelayCommand<ArtifactItemViewModel> EditArtifactCommand { get; }
    public IRelayCommand<ArtifactItemViewModel> DeleteArtifactCommand { get; }
    public IRelayCommand SaveArtifactCommand { get; }
    public IRelayCommand CancelEditCommand { get; }
    public IRelayCommand CloseCommand { get; }

    #endregion

    #region Events

    public event Action? CloseRequested;
    public event Action<int>? PhaseChanged;
    public event Action? ArtifactsChanged;
    public event Action<int>? DeleteArtifactRequested;
    public event Action? CreateArtifactRequested;
    public event Action<ArtifactItemViewModel>? EditArtifactRequested;

    #endregion

    #region Constructor

    public ArtifactAdministrationViewModel()
    {
        SelectPhaseCommand = new RelayCommand<string>(OnSelectPhase);
        CreateArtifactCommand = new RelayCommand(OnCreateArtifact);
        EditArtifactCommand = new RelayCommand<ArtifactItemViewModel>(OnEditArtifact);
        DeleteArtifactCommand = new RelayCommand<ArtifactItemViewModel>(OnDeleteArtifact);
        SaveArtifactCommand = new RelayCommand(OnSaveArtifact);
        CancelEditCommand = new RelayCommand(OnCancelEdit);
        CloseCommand = new RelayCommand(() => CloseRequested?.Invoke());
    }

    #endregion

    #region Command Handlers

    private void OnSelectPhase(string? phaseName)
    {
        if (string.IsNullOrEmpty(phaseName))
            return;

        SelectedPhaseName = phaseName;
        
        // Mapear nombre de fase a ID
        SelectedPhaseId = phaseName switch
        {
            "Inicio (Inception)" => 1,
            "Elaboración (Elaboration)" => 2,
            "Construcción (Construction)" => 3,
            "Transición (Transition)" => 4,
            _ => 1
        };

        PhaseChanged?.Invoke(SelectedPhaseId);
    }

    private void OnCreateArtifact()
    {
        CreateArtifactRequested?.Invoke();
    }

    private void OnEditArtifact(ArtifactItemViewModel? artifact)
    {
        if (artifact == null)
            return;

        EditArtifactRequested?.Invoke(artifact);
    }

    private void OnDeleteArtifact(ArtifactItemViewModel? artifact)
    {
        if (artifact == null)
            return;

        DeleteArtifactRequested?.Invoke(artifact.Id);
    }

    private void OnSaveArtifact()
    {
        if (string.IsNullOrWhiteSpace(EditName))
            return;

        if (SelectedArtifact != null)
        {
            // Actualizar artefacto existente
            SelectedArtifact.Name = EditName;
            SelectedArtifact.ArtifactType = EditArtifactType;
            SelectedArtifact.Mandatory = EditMandatory;
            SelectedArtifact.Description = EditDescription;
        }
        else
        {
            // Crear nuevo artefacto
            var newArtifact = new ArtifactItemViewModel
            {
                ProjectId = ProjectId,
                PhaseId = SelectedPhaseId,
                Name = EditName,
                ArtifactType = EditArtifactType,
                Mandatory = EditMandatory,
                Description = EditDescription,
                CurrentState = "PENDING",
                CurrentVersion = 0
            };
            Artifacts.Add(newArtifact);
        }

        ArtifactsChanged?.Invoke();
        OnCancelEdit();
    }

    private void OnCancelEdit()
    {
        IsEditMode = false;
        SelectedArtifact = null;
        EditName = string.Empty;
        EditArtifactType = null;
        EditMandatory = false;
        EditDescription = null;
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Inicializa el ViewModel con los datos del proyecto y fase.
    /// </summary>
    public void Initialize(int projectId, int phaseId, string phaseName)
    {
        ProjectId = projectId;
        SelectedPhaseId = phaseId;
        SelectedPhaseName = phaseName;
    }

    /// <summary>
    /// Carga los artefactos para la fase actual.
    /// </summary>
    public void LoadArtifactsForPhase(int phaseId)
    {
        // TODO: Implementar carga desde repositorio
        SelectedPhaseId = phaseId;
    }

    /// <summary>
    /// Notifica que los artefactos han cambiado.
    /// </summary>
    public void NotifyArtifactsChanged()
    {
        ArtifactsChanged?.Invoke();
    }

    #endregion
}

