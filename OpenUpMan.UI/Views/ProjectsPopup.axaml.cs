using Avalonia.Controls;
using Avalonia.Interactivity;
using OpenUpMan.UI.ViewModels;
using System;

namespace OpenUpMan.UI.Views;

public partial class ProjectsPopup : Window
{
    public ProjectsPopup()
    {
        InitializeComponent();
    }

    public ProjectsPopup(ProjectsPopupViewModel vm) : this()
    {
        DataContext = vm;
        vm.CloseRequested += OnCloseRequested;
    }

    private void OnCloseRequested()
    {
        this.Close();
    }
}
