using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Threading.Tasks;
using OpenUpMan.Services;

namespace OpenUpMan.UI.ViewModels;

public partial class DashboardViewModel : ViewModelBase
{
  private readonly IDashboardService _dashboardService;
  private readonly int _projectId;

  [ObservableProperty]
  private ObservableCollection<PhaseDisplayModel> _phases;

  [ObservableProperty]
  private ObservableCollection<IterationDisplayModel> _activeIterations;

  [ObservableProperty]
  private int _mandatoryArtifactsCount;

  [ObservableProperty]
  private int _totalMandatoryArtifacts;

  [ObservableProperty]
  private string _completionPercentageText;

  [ObservableProperty]
  private int _totalIterations;

  [ObservableProperty]
  private int _totalMicroincrements;

  [ObservableProperty]
  private double _averageMicroincrementsPerIteration;

  [ObservableProperty]
  private double _averageIterationDurationDays;

  [ObservableProperty]
  private bool _isLoading;

  [ObservableProperty]
  private ProjectStatisticsDto? _statistics;

  [ObservableProperty]
  private ArtifactProgressDto? _artifactProgress;

  public DashboardViewModel(IDashboardService dashboardService, int projectId)
  {
    _dashboardService = dashboardService;
    _projectId = projectId;
    
    Phases = new ObservableCollection<PhaseDisplayModel>();
    ActiveIterations = new ObservableCollection<IterationDisplayModel>();
    CompletionPercentageText = "0%";
    
    _ = LoadDataAsync();
  }

  // Constructor for design-time
  public DashboardViewModel() : this(null!, 0)
  {
    // Mock Data for Design
    Phases = new ObservableCollection<PhaseDisplayModel>
        {
            new PhaseDisplayModel("Inicio", true, false),
            new PhaseDisplayModel("Elaboración", false, true),
            new PhaseDisplayModel("Construcción", false, false),
            new PhaseDisplayModel("Transición", false, false)
        };

    ActiveIterations = new ObservableCollection<IterationDisplayModel>
        {
            new IterationDisplayModel("Iteración 2 - Elaboración", "Implementar los casos de uso principales del sistema y validar la arquitectura propuesta", 8, DateTime.Now.AddDays(-7), DateTime.Now.AddDays(7), 14)
        };

    MandatoryArtifactsCount = 12;
    TotalMandatoryArtifacts = 15;
    CompletionPercentageText = "80%";
    TotalIterations = 5;
    TotalMicroincrements = 23;
    AverageMicroincrementsPerIteration = 4.6;
    AverageIterationDurationDays = 18.5;
    
    Statistics = new ProjectStatisticsDto(
      TotalIterations: 5,
      TotalMicroincrements: 23,
      ActiveIterationsCount: 2,
      CompletedPhasesCount: 1,
      AverageMicroincrementsPerIteration: 4.6,
      AverageIterationDurationDays: 18.5
    );
    
    ArtifactProgress = new ArtifactProgressDto(
      MandatoryArtifactsRegistered: 12,
      TotalMandatoryArtifacts: 15,
      CompletionPercentage: 80.0
    );
  }

  private async Task LoadDataAsync()
  {
    if (_dashboardService == null) return;

    IsLoading = true;

    try
    {
      var data = await _dashboardService.GetDashboardDataAsync(_projectId);
      
      if (data != null)
      {
        // Load phases
        Phases.Clear();
        foreach (var phase in data.AllPhases)
        {
          Phases.Add(new PhaseDisplayModel(
            phase.Name, 
            phase.IsCompleted, 
            phase.IsCurrent));
        }

        // Load active iterations
        ActiveIterations.Clear();
        foreach (var iteration in data.ActiveIterations)
        {
          ActiveIterations.Add(new IterationDisplayModel(
            iteration.Name,
            iteration.Goal,
            iteration.MicroincrementsCount,
            iteration.StartDate,
            iteration.EndDate,
            iteration.DurationDays));
        }

        // Load artifact progress
        ArtifactProgress = data.ArtifactProgress;
        MandatoryArtifactsCount = data.ArtifactProgress.MandatoryArtifactsRegistered;
        TotalMandatoryArtifacts = data.ArtifactProgress.TotalMandatoryArtifacts;
        CompletionPercentageText = $"{data.ArtifactProgress.CompletionPercentage:F1}%";

        // Load statistics
        Statistics = data.Statistics;
        TotalIterations = data.Statistics.TotalIterations;
        TotalMicroincrements = data.Statistics.TotalMicroincrements;
        AverageMicroincrementsPerIteration = data.Statistics.AverageMicroincrementsPerIteration;
        AverageIterationDurationDays = data.Statistics.AverageIterationDurationDays;
      }
    }
    catch (Exception ex)
    {
      // Log error
      System.Diagnostics.Debug.WriteLine($"Error loading dashboard data: {ex.Message}");
    }
    finally
    {
      IsLoading = false;
    }
  }
}

public class PhaseDisplayModel
{
  public string Name { get; }
  public bool IsCompleted { get; }
  public bool IsCurrent { get; }
  public bool IsPending => !IsCompleted && !IsCurrent;
  public string StatusText => IsCompleted ? "Completada" : IsCurrent ? "En Progreso" : "Pendiente";
  public string StatusIcon => IsCompleted ? "✓" : IsCurrent ? "▶" : "○";

  public PhaseDisplayModel(string name, bool isCompleted, bool isCurrent)
  {
    Name = name;
    IsCompleted = isCompleted;
    IsCurrent = isCurrent;
  }
}

public class IterationDisplayModel
{
  public string Name { get; }
  public string Goal { get; }
  public int MicroincrementsCount { get; }
  public string DateRange { get; }
  public int DurationDays { get; }
  public string DurationText { get; }

  public IterationDisplayModel(string name, string goal, int microincrementsCount, DateTime start, DateTime end, int durationDays)
  {
    Name = name;
    Goal = goal;
    MicroincrementsCount = microincrementsCount;
    DateRange = $"{start:dd/MM} - {end:dd/MM}";
    DurationDays = durationDays;
    DurationText = $"{durationDays} días";
  }
}
