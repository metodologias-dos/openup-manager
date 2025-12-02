using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.DependencyInjection;
using OpenUpMan.Data;
using OpenUpMan.UI.ViewModels;

namespace OpenUpMan.UI.Views;

public partial class ProjectView : UserControl
{
    public ProjectView()
    {
        InitializeComponent();
        DataContextChanged += OnDataContextChanged;
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    private void OnDataContextChanged(object? sender, EventArgs e)
    {
        if (DataContext is ProjectViewModel vm)
        {
            // Avoid double subscription if DataContext is set multiple times
            vm.ManageArtifactsRequested -= OpenArtifactsWindow;
            vm.ManageArtifactsRequested += OpenArtifactsWindow;
        }
    }

    private async void OpenArtifactsWindow()
    {
        if (DataContext is not ProjectViewModel projectVm) return;

        var artifactRepo = Program.ServiceProvider.GetService<IArtifactRepository>();
        if (artifactRepo == null) return;

        var vm = new ArtifactsViewModel(artifactRepo);

        // Load artifacts for the current project. 
        // We default to "Incepción" for now as per requirements, 
        // but ideally we should know which phase is selected.
        await vm.LoadArtifactsAsync(projectVm.ProjectId, "Incepción", projectVm.CurrentUserId, projectVm.CurrentUserName);

        var window = new ArtifactsWindow
        {
            DataContext = vm
        };

        if (VisualRoot is Window parent)
        {
            await window.ShowDialog(parent);
        }
    }
}