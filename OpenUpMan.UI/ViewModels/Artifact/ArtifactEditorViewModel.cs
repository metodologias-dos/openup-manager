using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace OpenUpMan.UI.ViewModels;

/// <summary>
/// ViewModel para el diálogo de edición/creación de artefactos.
/// </summary>
public partial class ArtifactEditorViewModel : ViewModelBase
{
    #region Properties

    [ObservableProperty]
    private int? _artifactId;

    [ObservableProperty]
    private string _name = string.Empty;

    [ObservableProperty]
    private string? _artifactType;

    [ObservableProperty]
    private bool _isMandatory;

    [ObservableProperty]
    private string? _description;

    [ObservableProperty]
    private bool _canSave;

    public string WindowTitle => ArtifactId.HasValue ? "Editar Artefacto" : "Nuevo Artefacto";

    #endregion

    #region Commands

    public IRelayCommand SaveCommand { get; }
    public IRelayCommand CancelCommand { get; }

    #endregion

    #region Events

    public event Action? SaveRequested;
    public event Action? CancelRequested;

    #endregion

    #region Constructor

    public ArtifactEditorViewModel()
    {
        SaveCommand = new RelayCommand(OnSave, CanExecuteSave);
        CancelCommand = new RelayCommand(() => CancelRequested?.Invoke());

        // Validar cuando cambien las propiedades
        PropertyChanged += (s, e) =>
        {
            if (e.PropertyName == nameof(Name))
            {
                ValidateCanSave();
            }
        };
    }

    #endregion

    #region Command Handlers

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
    /// Inicializa el ViewModel para crear un nuevo artefacto.
    /// </summary>
    public void InitializeForNew()
    {
        ArtifactId = null;
        Name = string.Empty;
        ArtifactType = null;
        IsMandatory = false;
        Description = null;
        
        ValidateCanSave();
        OnPropertyChanged(nameof(WindowTitle));
    }

    /// <summary>
    /// Inicializa el ViewModel para editar un artefacto existente.
    /// </summary>
    public void InitializeForEdit(int artifactId, string name, string? artifactType, bool isMandatory, string? description)
    {
        ArtifactId = artifactId;
        Name = name;
        ArtifactType = artifactType;
        IsMandatory = isMandatory;
        Description = description;
        
        ValidateCanSave();
        OnPropertyChanged(nameof(WindowTitle));
    }

    #endregion

    #region Private Methods

    private void ValidateCanSave()
    {
        CanSave = !string.IsNullOrWhiteSpace(Name);
        SaveCommand.NotifyCanExecuteChanged();
    }

    #endregion
}

