using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace OpenUpMan.UI.ViewModels;

public partial class ProjectViewModel : ViewModelBase
{
    [ObservableProperty]
    private string _projectName = string.Empty;

    [ObservableProperty]
    private int _projectPercentage = 0;

    public IRelayCommand SaveCommand { get; }
    public IRelayCommand OpenCommand { get; }
    public IRelayCommand AddUserCommand { get; }
    public IRelayCommand BackCommand { get; }

    public event Action? BackRequested;

    public ProjectViewModel()
    {
        SaveCommand = new RelayCommand(() => { /* visual only */ });
        OpenCommand = new RelayCommand(() => { /* visual only */ });
        AddUserCommand = new RelayCommand(() => { /* visual only */ });
        BackCommand = new RelayCommand(GoBack);

        ProjectName = "Proyecto ejemplo";
        ProjectPercentage = 12;
    }

    private void GoBack()
    {
        BackRequested?.Invoke();
    }
}
