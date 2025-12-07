using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using System;

namespace OpenUpMan.UI.ViewModels;

public partial class DashboardViewModel : ViewModelBase
{
  [ObservableProperty]
  private ObservableCollection<PhaseDisplayModel> _phases;

  [ObservableProperty]
  private ObservableCollection<IterationDisplayModel> _activeIterations;

  [ObservableProperty]
  private int _mandatoryArtifactsCount;

  [ObservableProperty]
  private int _totalMandatoryArtifacts;

  public DashboardViewModel()
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
            new IterationDisplayModel("Iteración 1", "Definir arquitectura base", 5, DateTime.Now.AddDays(-14), DateTime.Now.AddDays(-1)),
            new IterationDisplayModel("Iteración 2", "Implementar casos de uso core", 2, DateTime.Now, DateTime.Now.AddDays(14))
        };

    MandatoryArtifactsCount = 12;
    TotalMandatoryArtifacts = 15;
  }
}

public class PhaseDisplayModel
{
  public string Name { get; }
  public bool IsCompleted { get; }
  public bool IsCurrent { get; }
  public bool IsPending => !IsCompleted && !IsCurrent;

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

  public IterationDisplayModel(string name, string goal, int microincrementsCount, DateTime start, DateTime end)
  {
    Name = name;
    Goal = goal;
    MicroincrementsCount = microincrementsCount;
    DateRange = $"{start:dd/MM} - {end:dd/MM}";
  }
}
