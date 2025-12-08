using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using OpenUpMan.Data;
using OpenUpMan.Services;

namespace OpenUpMan.UI.ViewModels;

/// <summary>
/// ViewModel que representa una versión de un artefacto en el historial.
/// </summary>
public partial class ArtifactVersionItemViewModel : ViewModelBase
{
    [ObservableProperty]
    private int _id;

    [ObservableProperty]
    private int _artifactId;

    [ObservableProperty]
    private int _versionNumber;

    [ObservableProperty]
    private string _authorName = "Desconocido";

    [ObservableProperty]
    private DateTime _createdAt;

    [ObservableProperty]
    private string? _notes;

    [ObservableProperty]
    private bool _hasFile;

    [ObservableProperty]
    private string? _fileMime;

    [ObservableProperty]
    private string? _buildInfo;

    public string CreatedAtFormatted => CreatedAt.ToString("dd/MM/yyyy HH:mm");
    
    public string VersionDisplay => $"v{VersionNumber}";
    
    public string FileTypeDisplay => HasFile 
        ? (FileMime?.Split('/').LastOrDefault() ?? "Archivo") 
        : "URL/Enlace";
}

/// <summary>
/// ViewModel para la ventana de historial de artefactos.
/// Muestra todas las versiones de un artefacto con información de autor, fecha y permite previsualizar.
/// </summary>
public partial class ArtifactHistoryViewModel : ViewModelBase
{
    #region Properties

    [ObservableProperty]
    private int _artifactId;

    [ObservableProperty]
    private string _artifactName = string.Empty;

    [ObservableProperty]
    private int _currentVersion;

    [ObservableProperty]
    private ObservableCollection<ArtifactVersionItemViewModel> _versions = new();

    [ObservableProperty]
    private ArtifactVersionItemViewModel? _selectedVersion;

    #endregion

    #region Commands

    public IRelayCommand<ArtifactVersionItemViewModel> PreviewVersionCommand { get; }
    public IRelayCommand CloseCommand { get; }

    #endregion

    #region Events

    public event Action? CloseRequested;
    public event Action<int>? PreviewVersionRequested;

    #endregion

    #region Constructor

    public ArtifactHistoryViewModel()
    {
        PreviewVersionCommand = new RelayCommand<ArtifactVersionItemViewModel>(OnPreviewVersion);
        CloseCommand = new RelayCommand(() => CloseRequested?.Invoke());
    }

    #endregion

    #region Command Handlers

    private void OnPreviewVersion(ArtifactVersionItemViewModel? version)
    {
        if (version == null)
            return;

        PreviewVersionRequested?.Invoke(version.Id);
    }


    #endregion

    #region Public Methods

    /// <summary>
    /// Carga el historial de versiones para el artefacto especificado.
    /// </summary>
    public void LoadVersionHistory(int artifactId, string artifactName)
    {
        ArtifactId = artifactId;
        ArtifactName = artifactName;
    }

    /// <summary>
    /// Carga el historial de versiones de forma asíncrona.
    /// Se llama desde la ventana después de inicializar el ViewModel.
    /// </summary>
    public async Task LoadVersionHistoryAsync(IArtifactVersionService artifactVersionService, IUserRepository userRepo)
    {
        Versions.Clear();

        try
        {
            var versions = await artifactVersionService.GetVersionHistoryAsync(ArtifactId);
            if (versions != null)
            {
                foreach (var version in versions.OrderByDescending(v => v.VersionNumber))
                {
                    // Get author name
                    var author = version.CreatedBy.HasValue ? await userRepo.GetByIdAsync(version.CreatedBy.Value) : null;
                    var authorName = author?.Username ?? "Desconocido";

                    Versions.Add(new ArtifactVersionItemViewModel
                    {
                        Id = version.Id,
                        ArtifactId = version.ArtifactId,
                        VersionNumber = version.VersionNumber,
                        AuthorName = authorName,
                        CreatedAt = version.CreatedAt,
                        Notes = version.Notes,
                        HasFile = version.FileBlob != null && version.FileBlob.Length > 0,
                        FileMime = version.FileMime,
                        BuildInfo = version.BuildInfo
                    });
                }

                if (Versions.Any())
                {
                    CurrentVersion = Versions.Max(v => v.VersionNumber);
                }
            }
        }
        catch (Exception ex)
        {
            // Log error or show message
            System.Diagnostics.Debug.WriteLine($"Error loading artifact history: {ex.Message}");
        }
    }

    #endregion
}

