using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using OpenUpMan.Data;
using OpenUpMan.Domain;

namespace OpenUpMan.UI.ViewModels;

public partial class ArtifactDisplayItem : ObservableObject
{
  public int Id { get; set; }

  [ObservableProperty]
  private int _number;

  [ObservableProperty]
  private string _name = string.Empty;

  [ObservableProperty]
  [NotifyPropertyChangedFor(nameof(MandatoryText))]
  private bool _isMandatory;

  [ObservableProperty]
  [NotifyPropertyChangedFor(nameof(Status))]
  private bool _isRegistered;

  public string Status => IsRegistered ? "Registrado" : "No registrado";
  public string MandatoryText => IsMandatory ? "Sí" : "No";
}
public partial class ArtifactsViewModel : ViewModelBase
{
  private readonly IArtifactRepository _artifactRepository;
  private int _currentProjectId;
  private int _currentPhaseId;
  private string _currentPhaseName = string.Empty;
  private int _currentUserId;
  private string _currentUserName = string.Empty;

  public event System.Action<int, int, string>? EditArtifactRequested;

  [ObservableProperty]
  private string _phaseName = "Fase";

  public ObservableCollection<ArtifactDisplayItem> Artifacts { get; } = new();

  public IRelayCommand<ArtifactDisplayItem> EditCommand { get; }

  public ArtifactsViewModel(IArtifactRepository artifactRepository)
  {
    _artifactRepository = artifactRepository;
    EditCommand = new RelayCommand<ArtifactDisplayItem>(EditArtifact);
  }

  // Constructor for design-time or no-di scenarios (optional, but good for previewer if needed)
  public ArtifactsViewModel()
  {
    _artifactRepository = null!;
    EditCommand = new RelayCommand<ArtifactDisplayItem>(item => { });
  }

  private void EditArtifact(ArtifactDisplayItem? item)
  {
    if (item == null) return;
    EditArtifactRequested?.Invoke(item.Id, _currentUserId, _currentUserName);
  }

  public async Task LoadArtifactsAsync(int projectId, int phaseId, string phaseName, int userId, string userName)
  {
    _currentProjectId = projectId;
    _currentPhaseId = phaseId;
    _currentPhaseName = phaseName;
    _currentUserId = userId;
    _currentUserName = userName;

    PhaseName = phaseName;
    Artifacts.Clear();
    
    if (_artifactRepository == null) return;

    // Filter artifacts by phase ID
    var artifacts = await _artifactRepository.GetByPhaseIdAsync(phaseId);

    int index = 1;
    foreach (var artifact in artifacts)
    {
      Artifacts.Add(new ArtifactDisplayItem
      {
        Id = artifact.Id,
        Number = index++,
        Name = artifact.Name,
        IsMandatory = artifact.Mandatory,
        IsRegistered = artifact.CurrentState == "REGISTERED"
      });
    }
  }

  public async Task RefreshAsync()
  {
    if (_currentProjectId != 0 && _currentPhaseId != 0)
    {
      await LoadArtifactsAsync(_currentProjectId, _currentPhaseId, _currentPhaseName, _currentUserId, _currentUserName);
    }
  }
}