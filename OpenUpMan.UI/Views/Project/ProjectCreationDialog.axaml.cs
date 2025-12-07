using Avalonia.Controls;
using OpenUpMan.UI.ViewModels;

namespace OpenUpMan.UI.Views;

public partial class ProjectCreationDialog : Window
{
    public ProjectCreationDialog()
    {
        InitializeComponent();
    }

    public ProjectCreationDialog(ProjectCreationDialogViewModel vm) : this()
    {
        DataContext = vm;
        vm.CloseRequested += OnCloseRequested;
    }

    private void OnCloseRequested()
    {
        this.Close();
    }
}

