using System;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using OpenUpMan.Domain;

namespace OpenUpMan.UI.ViewModels;

public partial class ProjectViewModel : ViewModelBase
{
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
    private ObservableCollection<IterationItemViewModel> _iterations = new();

    [ObservableProperty]
    private ObservableCollection<MicroincrementItemViewModel> _microincrements = new();

    [ObservableProperty]
    private ObservableCollection<ArtifactItemViewModel> _phaseArtifacts = new();

    public IRelayCommand SaveCommand { get; }
    public IRelayCommand OpenCommand { get; }
    public IRelayCommand AddUserCommand { get; }
    public IRelayCommand OpenDashboardCommand { get; }
    public IRelayCommand BackCommand { get; }
    public IRelayCommand ManageArtifactsCommand { get; }
    public IRelayCommand<string> SelectPhaseCommand { get; }
    public IRelayCommand CreateIterationCommand { get; }
    public IRelayCommand<ArtifactItemViewModel> RegisterArtifactChangeCommand { get; }
    public IRelayCommand<ArtifactItemViewModel> ViewArtifactHistoryCommand { get; }
    public IRelayCommand<int?> PreviewArtifactCommand { get; }

    public event Action? BackRequested;
    public event Action? ManageArtifactsRequested;
    public event Action? CreateIterationRequested;
    public event Action? OpenDashboardRequested;
    public event Action<int, string>? ArtifactChangeRequested;
    public event Action<int, string>? ArtifactHistoryRequested;
    public event Action<int>? ArtifactPreviewRequested;
    public event Action? MicroincrementsChanged;

    public ProjectViewModel()
    {
        SaveCommand = new RelayCommand(() => { /* visual only */ });
        OpenCommand = new RelayCommand(() => { /* visual only */ });
        AddUserCommand = new RelayCommand(() => { /* visual only */ });
        OpenDashboardCommand = new RelayCommand(() => OpenDashboardRequested?.Invoke());
        BackCommand = new RelayCommand(GoBack);
        ManageArtifactsCommand = new RelayCommand(() => ManageArtifactsRequested?.Invoke());
        SelectPhaseCommand = new RelayCommand<string>(SelectPhase);
        CreateIterationCommand = new RelayCommand(() => CreateIterationRequested?.Invoke());

        RegisterArtifactChangeCommand = new RelayCommand<ArtifactItemViewModel>(artifact =>
        {
            if (artifact != null)
                ArtifactChangeRequested?.Invoke(artifact.Id, artifact.Name);
        });

        ViewArtifactHistoryCommand = new RelayCommand<ArtifactItemViewModel>(artifact =>
        {
            if (artifact != null)
                ArtifactHistoryRequested?.Invoke(artifact.Id, artifact.Name);
        });

        PreviewArtifactCommand = new RelayCommand<int?>(artifactId =>
        {
            if (artifactId.HasValue)
                ArtifactPreviewRequested?.Invoke(artifactId.Value);
        });

        ProjectName = "Proyecto ejemplo";
        ProjectPercentage = 12;
    }

    private void SelectPhase(string? phaseName)
    {
        if (!string.IsNullOrEmpty(phaseName))
        {
            CurrentPhaseName = phaseName;
        }
    }

    private void GoBack()
    {
        BackRequested?.Invoke();
    }

    public void NotifyMicroincrementsChanged()
    {
        MicroincrementsChanged?.Invoke();
    }
}

