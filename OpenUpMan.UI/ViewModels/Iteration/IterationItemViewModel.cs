using System;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace OpenUpMan.UI.ViewModels;

public partial class IterationItemViewModel : ViewModelBase
{
    [ObservableProperty]
    private int _id;

    [ObservableProperty]
    private int _phaseId;

    [ObservableProperty]
    private string _name = string.Empty;

    [ObservableProperty]
    private string? _goal;

    [ObservableProperty]
    private DateTime? _startDate;

    [ObservableProperty]
    private DateTime? _endDate;

    [ObservableProperty]
    private int _completionPercentage;

    [ObservableProperty]
    private bool _isActive;

    [ObservableProperty]
    private ObservableCollection<MicroincrementItemViewModel> _microincrements = new();

    public string ActiveIndicator => IsActive ? "⭐" : "";
}

