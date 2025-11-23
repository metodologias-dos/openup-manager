using Avalonia.Controls;
using OpenUpMan.UI.ViewModels;

namespace OpenUpMan.UI.Views;

public partial class ProjectDialog : Window
{
    public ProjectDialog()
    {
        InitializeComponent();
    }

    public ProjectDialog(ProjectDialogViewModel vm) : this()
    {
        DataContext = vm;
        vm.CloseRequested += OnCloseRequested;
    }

    private void OnCloseRequested()
    {
        this.Close();
    }
}

