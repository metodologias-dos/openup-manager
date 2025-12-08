using System;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace OpenUpMan.UI.ViewModels;

/// <summary>
/// ViewModel para la vista del proyecto.
/// Gestiona el estado y los comandos relacionados con el proyecto, fases, iteraciones y artefactos.
/// </summary>
public partial class ProjectViewModel : ViewModelBase
{
    #region Properties

    [ObservableProperty]
    private int _projectId;

    [ObservableProperty]
    private string _projectName = string.Empty;

    [ObservableProperty]
    private int _currentUserId;

    [ObservableProperty]
    private string _currentUserName = string.Empty;

    [ObservableProperty]
    private int _projectPercentage;

    [ObservableProperty]
    private int _currentPhaseId;

    [ObservableProperty]
    private string _currentPhaseName = "Inicio (Inception)";

    [ObservableProperty]
    private string _currentPhaseStatus = "PENDING";

    [ObservableProperty]
    private string _currentPhaseStatusDisplay = "Pendiente";

    [ObservableProperty]
    private int _selectedPhaseIndex = 0;

    [ObservableProperty]
    private ObservableCollection<IterationItemViewModel> _iterations = new();

    [ObservableProperty]
    private ObservableCollection<MicroincrementItemViewModel> _microincrements = new();

    [ObservableProperty]
    private ObservableCollection<ArtifactItemViewModel> _phaseArtifacts = new();

    #endregion

    #region Commands

    public IRelayCommand SaveCommand { get; }
    public IRelayCommand OpenCommand { get; }
    public IRelayCommand AddUserCommand { get; }
    public IRelayCommand OpenDashboardCommand { get; }
    public IRelayCommand BackCommand { get; }
    public IRelayCommand ManageArtifactsCommand { get; }
    public IRelayCommand<string> SelectPhaseCommand { get; }
    public IRelayCommand CreateIterationCommand { get; }
    public IRelayCommand<IterationItemViewModel> ActivateIterationCommand { get; }
    public IRelayCommand<IterationItemViewModel> ViewIterationDetailsCommand { get; }
    public IRelayCommand<ArtifactItemViewModel> RegisterArtifactChangeCommand { get; }
    public IRelayCommand<ArtifactItemViewModel> ViewArtifactHistoryCommand { get; }
    public IRelayCommand<int?> PreviewArtifactCommand { get; }
    public IRelayCommand StartPhaseCommand { get; }
    public IRelayCommand EndPhaseCommand { get; }

    #endregion

    #region Events

    /// <summary>
    /// Se dispara cuando el usuario solicita volver a la vista anterior.
    /// </summary>
    public event Action? BackRequested;

    /// <summary>
    /// Se dispara cuando el usuario solicita gestionar los artefactos.
    /// </summary>
    public event Action? ManageArtifactsRequested;

    /// <summary>
    /// Se dispara cuando el usuario solicita crear una nueva iteración.
    /// </summary>
    public event Action? CreateIterationRequested;

    /// <summary>
    /// Se dispara cuando el usuario solicita abrir el tablero del proyecto.
    /// </summary>
    public event Action? OpenDashboardRequested;

    /// <summary>
    /// Se dispara cuando el usuario solicita registrar un cambio en un artefacto.
    /// Parámetros: (artifactId, artifactName)
    /// </summary>
    public event Action<int, string>? ArtifactChangeRequested;

    /// <summary>
    /// Se dispara cuando el usuario solicita ver el historial de un artefacto.
    /// Parámetros: (artifactId, artifactName)
    /// </summary>
    public event Action<int, string>? ArtifactHistoryRequested;

    /// <summary>
    /// Se dispara cuando el usuario solicita previsualizar un artefacto.
    /// Parámetro: artifactId
    /// </summary>
    public event Action<int>? ArtifactPreviewRequested;

    /// <summary>
    /// Se dispara cuando se solicita activar una iteración.
    /// Parámetro: iterationId
    /// </summary>
    public event Action<int>? ActivateIterationRequested;

    /// <summary>
    /// Se dispara cuando se solicita ver detalles de una iteración.
    /// Parámetro: iteration
    /// </summary>
    public event Action<IterationItemViewModel>? ViewIterationDetailsRequested;

    /// <summary>
    /// Se dispara cuando cambia la fase seleccionada.
    /// </summary>
    public event Action? PhaseChanged;

    /// <summary>
    /// Se dispara cuando hay cambios en los microincrementos.
    /// </summary>
    public event Action? MicroincrementsChanged;

    /// <summary>
    /// Se dispara cuando el usuario solicita iniciar una fase.
    /// </summary>
    public event Action? StartPhaseRequested;

    /// <summary>
    /// Se dispara cuando el usuario solicita finalizar una fase.
    /// </summary>
    public event Action? EndPhaseRequested;

    #endregion

    #region Constructor

    public ProjectViewModel()
    {
        // Inicializar comandos
        SaveCommand = new RelayCommand(OnSave);
        OpenCommand = new RelayCommand(OnOpen);
        AddUserCommand = new RelayCommand(OnAddUser);
        OpenDashboardCommand = new RelayCommand(() => OpenDashboardRequested?.Invoke());
        BackCommand = new RelayCommand(() => BackRequested?.Invoke());
        ManageArtifactsCommand = new RelayCommand(() => ManageArtifactsRequested?.Invoke());
        SelectPhaseCommand = new RelayCommand<string>(OnSelectPhase);
        CreateIterationCommand = new RelayCommand(() => CreateIterationRequested?.Invoke());
        ActivateIterationCommand = new RelayCommand<IterationItemViewModel>(OnActivateIteration);
        ViewIterationDetailsCommand = new RelayCommand<IterationItemViewModel>(OnViewIterationDetails);
        RegisterArtifactChangeCommand = new RelayCommand<ArtifactItemViewModel>(OnRegisterArtifactChange);
        ViewArtifactHistoryCommand = new RelayCommand<ArtifactItemViewModel>(OnViewArtifactHistory);
        PreviewArtifactCommand = new RelayCommand<int?>(OnPreviewArtifact);
        StartPhaseCommand = new RelayCommand(OnStartPhase);
        EndPhaseCommand = new RelayCommand(OnEndPhase);

        // Valores por defecto
        ProjectName = "Proyecto ejemplo";
        ProjectPercentage = 12;
    }

    #endregion

    #region Command Handlers

    private void OnSave()
    {
        // TODO: Implementar guardado del proyecto
    }

    private void OnOpen()
    {
        // TODO: Implementar apertura del proyecto
    }

    private void OnAddUser()
    {
        // TODO: Implementar agregar usuario al proyecto
    }

    private void OnSelectPhase(string? phaseName)
    {
        if (!string.IsNullOrEmpty(phaseName))
        {
            CurrentPhaseName = phaseName;
            
            // Update phase index for consistency
            SelectedPhaseIndex = phaseName switch
            {
                "Inicio (Inception)" => 0,
                "Elaboración (Elaboration)" => 1,
                "Construcción (Construction)" => 2,
                "Transición (Transition)" => 3,
                _ => 0
            };
            
            PhaseChanged?.Invoke();
        }
    }

    private void OnActivateIteration(IterationItemViewModel? iteration)
    {
        if (iteration != null)
        {
            ActivateIterationRequested?.Invoke(iteration.Id);
        }
    }

    private void OnViewIterationDetails(IterationItemViewModel? iteration)
    {
        if (iteration != null)
        {
            ViewIterationDetailsRequested?.Invoke(iteration);
        }
    }

    private void OnRegisterArtifactChange(ArtifactItemViewModel? artifact)
    {
        if (artifact != null)
        {
            ArtifactChangeRequested?.Invoke(artifact.Id, artifact.Name);
        }
    }

    private void OnViewArtifactHistory(ArtifactItemViewModel? artifact)
    {
        if (artifact != null)
        {
            ArtifactHistoryRequested?.Invoke(artifact.Id, artifact.Name);
        }
    }

    private void OnPreviewArtifact(int? artifactId)
    {
        if (artifactId.HasValue)
        {
            ArtifactPreviewRequested?.Invoke(artifactId.Value);
        }
    }

    private void OnStartPhase()
    {
        StartPhaseRequested?.Invoke();
    }

    private void OnEndPhase()
    {
        EndPhaseRequested?.Invoke();
    }

    #endregion

    #region Property Changed Handlers

    /// <summary>
    /// Se ejecuta cuando cambia el índice de la fase seleccionada.
    /// Actualiza el nombre de la fase actual y notifica el cambio.
    /// </summary>
    partial void OnSelectedPhaseIndexChanged(int value)
    {
        string phaseName = value switch
        {
            0 => "Inicio (Inception)",
            1 => "Elaboración (Elaboration)",
            2 => "Construcción (Construction)",
            3 => "Transición (Transition)",
            _ => "Inicio (Inception)"
        };

        if (CurrentPhaseName != phaseName)
        {
            CurrentPhaseName = phaseName;
            PhaseChanged?.Invoke();
        }
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Notifica que los microincrementos han cambiado y deben recargarse.
    /// </summary>
    public void NotifyMicroincrementsChanged()
    {
        MicroincrementsChanged?.Invoke();
    }

    /// <summary>
    /// Actualiza el estado de la fase actual
    /// </summary>
    public void UpdatePhaseStatus(string status)
    {
        CurrentPhaseStatus = status;
        CurrentPhaseStatusDisplay = status switch
        {
            "PENDING" => "Pendiente",
            "IN_PROGRESS" => "En progreso",
            "DONE" => "Terminado",
            _ => "Pendiente"
        };
    }

    #endregion
}

