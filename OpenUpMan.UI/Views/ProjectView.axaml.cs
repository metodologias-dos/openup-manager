using System;
using System.Linq;
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
        var phaseRepo = Program.ServiceProvider.GetService<IPhaseRepository>();
        if (artifactRepo == null || phaseRepo == null) return;

        // Get the phase ID for the current phase name
        var phases = (await phaseRepo.GetByProjectIdAsync(projectVm.ProjectId)).ToList();
        var currentPhase = phases.FirstOrDefault(p => p.Name == projectVm.CurrentPhaseName);
        
        if (currentPhase == null)
        {
            // Fallback to first phase if not found
            currentPhase = phases.FirstOrDefault();
            if (currentPhase == null) return;
        }

        var vm = new ArtifactsViewModel(artifactRepo);

        // Load artifacts filtered by the current phase
        await vm.LoadArtifactsAsync(projectVm.ProjectId, currentPhase.Id, projectVm.CurrentPhaseName, projectVm.CurrentUserId, projectVm.CurrentUserName);

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