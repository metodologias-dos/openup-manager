using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using OpenUpMan.Services;

namespace OpenUpMan.UI.ViewModels;

/// <summary>
/// ViewModel para la ventana de registro de artefactos.
/// Permite registrar una nueva versión de un artefacto (documento o URL) y crear automáticamente un microincremento.
/// </summary>
public partial class RegisterArtifactViewModel : ViewModelBase
{
    #region Services

    private readonly IArtifactVersionService _artifactVersionService;
    private readonly IMicroincrementService _microincrementService;
    private readonly IIterationService _iterationService;

    #endregion

    #region Properties

    [ObservableProperty]
    private int _artifactId;

    [ObservableProperty]
    private string _artifactName = string.Empty;

    [ObservableProperty]
    private int _projectId;

    [ObservableProperty]
    private int _phaseId;

    [ObservableProperty]
    private int _currentUserId;

    [ObservableProperty]
    private string _currentUserName = string.Empty;

    [ObservableProperty]
    private int? _activeIterationId;

    [ObservableProperty]
    private string? _activeIterationName;

    [ObservableProperty]
    private bool _hasActiveIteration;

    [ObservableProperty]
    private string? _notes;

    [ObservableProperty]
    private bool _isUploadingFile = true; // true = subir archivo, false = usar URL

    [ObservableProperty]
    private string? _filePath;

    [ObservableProperty]
    private string? _fileName;

    [ObservableProperty]
    private string? _url;

    [ObservableProperty]
    private bool _canSave;

    #endregion

    #region Commands

    public IRelayCommand SelectFileCommand { get; }
    public IRelayCommand ToggleUploadTypeCommand { get; }
    public IRelayCommand SaveCommand { get; }
    public IRelayCommand CancelCommand { get; }

    #endregion

    #region Events

    public event Action? CloseRequested;
    public event Action? SaveRequested;
    public event Action? SelectFileRequested;

    #endregion

    #region Constructor

    public RegisterArtifactViewModel(
        IArtifactVersionService artifactVersionService,
        IMicroincrementService microincrementService,
        IIterationService iterationService)
    {
        _artifactVersionService = artifactVersionService;
        _microincrementService = microincrementService;
        _iterationService = iterationService;

        SelectFileCommand = new RelayCommand(OnSelectFile);
        ToggleUploadTypeCommand = new RelayCommand(OnToggleUploadType);
        SaveCommand = new RelayCommand(OnSave, CanExecuteSave);
        CancelCommand = new RelayCommand(() => CloseRequested?.Invoke());

        // Suscribirse a cambios para validar
        PropertyChanged += (s, e) =>
        {
            if (e.PropertyName == nameof(IsUploadingFile) ||
                e.PropertyName == nameof(FilePath) ||
                e.PropertyName == nameof(Url) ||
                e.PropertyName == nameof(HasActiveIteration))
            {
                ValidateCanSave();
            }
        };
    }

    #endregion

    #region Command Handlers

    private void OnSelectFile()
    {
        SelectFileRequested?.Invoke();
    }

    private void OnToggleUploadType()
    {
        IsUploadingFile = !IsUploadingFile;
        
        // Limpiar campos al cambiar de tipo
        if (IsUploadingFile)
        {
            Url = null;
        }
        else
        {
            FilePath = null;
            FileName = null;
        }
    }

    private bool CanExecuteSave()
    {
        return CanSave;
    }

    private void OnSave()
    {
        if (!CanSave)
            return;

        SaveRequested?.Invoke();
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Inicializa el ViewModel con los datos del artefacto y carga la iteración activa.
    /// </summary>
    public async Task InitializeAsync(int artifactId, string artifactName, int projectId, int phaseId, int currentUserId)
    {
        ArtifactId = artifactId;
        ArtifactName = artifactName;
        ProjectId = projectId;
        PhaseId = phaseId;
        CurrentUserId = currentUserId;

        // Load active iteration for the phase
        var activeIteration = await _iterationService.GetActiveIterationByPhaseIdAsync(phaseId);
        if (activeIteration != null)
        {
            ActiveIterationId = activeIteration.Id;
            ActiveIterationName = activeIteration.Name;
            HasActiveIteration = true;
        }
        else
        {
            HasActiveIteration = false;
        }
        
        ValidateCanSave();
    }

    /// <summary>
    /// Inicializa el ViewModel con los datos del artefacto y la iteración activa.
    /// </summary>
    public void Initialize(int artifactId, string artifactName, int projectId, int phaseId, 
                          int currentUserId, string currentUserName, 
                          int? activeIterationId, string? activeIterationName)
    {
        ArtifactId = artifactId;
        ArtifactName = artifactName;
        ProjectId = projectId;
        PhaseId = phaseId;
        CurrentUserId = currentUserId;
        CurrentUserName = currentUserName;
        ActiveIterationId = activeIterationId;
        ActiveIterationName = activeIterationName;
        HasActiveIteration = activeIterationId.HasValue;
        
        ValidateCanSave();
    }

    /// <summary>
    /// Establece el archivo seleccionado.
    /// </summary>
    public void SetSelectedFile(string filePath, string fileName)
    {
        FilePath = filePath;
        FileName = fileName;
        ValidateCanSave();
    }

    #endregion

    #region Private Methods

    private void ValidateCanSave()
    {
        // Se puede guardar si:
        // 1. Hay una iteración activa
        // 2. Se ha seleccionado un archivo O se ha ingresado una URL
        CanSave = HasActiveIteration && 
                  ((IsUploadingFile && !string.IsNullOrWhiteSpace(FilePath)) ||
                   (!IsUploadingFile && !string.IsNullOrWhiteSpace(Url)));
        
        SaveCommand.NotifyCanExecuteChanged();
    }

    #endregion
}

