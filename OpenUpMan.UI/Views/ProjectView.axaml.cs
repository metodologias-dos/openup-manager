using System;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.DependencyInjection;
using OpenUpMan.Data;
using OpenUpMan.Services; // added for IIterationService
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

            // Subscribe to create iteration requests
            vm.CreateIterationRequested -= OpenCreateIterationDialog;
            vm.CreateIterationRequested += OpenCreateIterationDialog;

            // Load existing iterations for the project
            _ = LoadIterationsForProjectAsync(vm);
        }
    }

    private async System.Threading.Tasks.Task LoadIterationsForProjectAsync(ProjectViewModel vm)
    {
        var iterationService = Program.ServiceProvider.GetService<IIterationService>();
        var phaseRepo = Program.ServiceProvider.GetService<IPhaseRepository>();
        if (iterationService == null || phaseRepo == null) return;

        try
        {
            // Load phases to find iterations by phase
            var phases = (await phaseRepo.GetByProjectIdAsync(vm.ProjectId)).ToList();
            vm.Iterations.Clear();
            foreach (var phase in phases)
            {
                var iterations = await iterationService.GetIterationsByPhaseIdAsync(phase.Id);
                foreach (var it in iterations)
                {
                    vm.Iterations.Add(it);
                }
            }
        }
        catch
        {
            // ignore for now - visual only
        }
    }

    private async void OpenCreateIterationDialog()
    {
        if (DataContext is not ProjectViewModel projectVm) return;

        var phaseRepo = Program.ServiceProvider.GetService<IPhaseRepository>();
        var iterationService = Program.ServiceProvider.GetService<IIterationService>();
        if (phaseRepo == null || iterationService == null) return;

        var phases = (await phaseRepo.GetByProjectIdAsync(projectVm.ProjectId)).ToList();
        if (!phases.Any())
        {
            // no phases available for this project
            return;
        }

        var dialog = new IterationCreateWindow();
        dialog.SetPhases(phases);

        if (VisualRoot is Window parent)
        {
            var result = await dialog.ShowDialog<object?>(parent);
            if (result is OpenUpMan.Domain.Iteration created)
            {
                // persist using service
                var sr = await iterationService.CreateIterationAsync(created.PhaseId, created.Name, created.Goal, created.StartDate, created.EndDate);
                if (sr.Success && sr.Iteration != null)
                {
                    projectVm.Iterations.Add(sr.Iteration);
                }
            }
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