 using System;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using OpenUpMan.UI.ViewModels;

namespace OpenUpMan.UI.Views.Artifact;

public partial class ArtifactEditorWindow : Window
{
    public ArtifactEditorWindow()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public void SetViewModel(ArtifactEditorViewModel viewModel)
    {
        DataContext = viewModel;
        
        // Suscribirse a eventos
        viewModel.SaveRequested += () => Close(true);
        viewModel.CancelRequested += () => Close(false);
    }
}

