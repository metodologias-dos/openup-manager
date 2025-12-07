using System;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.DependencyInjection;
using OpenUpMan.Data;
using OpenUpMan.Services; // added for IIterationService
using OpenUpMan.UI.ViewModels;
using OpenUpMan.UI.Views.Artifact;

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

            vm.OpenDashboardRequested -= OpenDashboardWindow;
            vm.OpenDashboardRequested += OpenDashboardWindow;

            // Subscribe to artifact-related events
            vm.ArtifactChangeRequested -= OpenRegisterArtifactChangeWindow;
            vm.ArtifactChangeRequested += OpenRegisterArtifactChangeWindow;

            vm.ArtifactHistoryRequested -= OpenArtifactHistoryWindow;
            vm.ArtifactHistoryRequested += OpenArtifactHistoryWindow;

            vm.ArtifactPreviewRequested -= PreviewArtifact;
            vm.ArtifactPreviewRequested += PreviewArtifact;

            // Subscribe to activate iteration requests
            vm.ActivateIterationRequested -= ActivateIteration;
            vm.ActivateIterationRequested += ActivateIteration;

            // Subscribe to view iteration details requests
            vm.ViewIterationDetailsRequested -= OpenIterationDetailsWindow;
            vm.ViewIterationDetailsRequested += OpenIterationDetailsWindow;

            // Subscribe to phase change events
            vm.PhaseChanged -= OnPhaseChanged;
            vm.PhaseChanged += OnPhaseChanged;

            // Load existing iterations for the project
            _ = LoadIterationsForProjectAsync(vm);
            
            // Load artifacts for current phase
            _ = LoadArtifactsForCurrentPhaseAsync(vm);
        }
    }

    private async void OnPhaseChanged()
    {
        if (DataContext is ProjectViewModel vm)
        {
            await LoadIterationsForCurrentPhaseAsync(vm);
            await LoadArtifactsForCurrentPhaseAsync(vm);
        }
    }

    private async void ActivateIteration(int iterationId)
    {
        if (DataContext is not ProjectViewModel vm) return;

        var iterationService = Program.ServiceProvider.GetService<IIterationService>();
        if (iterationService == null) return;

        try
        {
            var result = await iterationService.ActivateIterationAsync(iterationId);
            if (result.Success)
            {
                // Reload iterations to reflect the change
                await LoadIterationsForCurrentPhaseAsync(vm);
            }
        }
        catch
        {
            // Handle error - could show a message to user
        }
    }

    private void OpenIterationDetailsWindow(IterationItemViewModel iteration)
    {
        var window = new IterationDetailsWindow
        {
            DataContext = iteration
        };

        if (VisualRoot is Window parent)
        {
            window.ShowDialog(parent);
        }
        else
        {
            window.Show();
        }
    }

    private async System.Threading.Tasks.Task LoadIterationsForCurrentPhaseAsync(ProjectViewModel vm)
    {
        var iterationService = Program.ServiceProvider.GetService<IIterationService>();
        var phaseRepo = Program.ServiceProvider.GetService<IPhaseRepository>();
        var microincrementService = Program.ServiceProvider.GetService<IMicroincrementService>();
        var userRepo = Program.ServiceProvider.GetService<IUserRepository>();
        var artifactRepo = Program.ServiceProvider.GetService<IArtifactRepository>();
        
        if (iterationService == null || phaseRepo == null || microincrementService == null) return;

        try
        {
            // Load phases to find the current phase
            var phases = (await phaseRepo.GetByProjectIdAsync(vm.ProjectId)).ToList();
            var currentPhase = phases.FirstOrDefault(p => p.Name == vm.CurrentPhaseName);
            
            if (currentPhase == null) return;

            vm.Iterations.Clear();
            
            var iterations = (await iterationService.GetIterationsByPhaseIdAsync(currentPhase.Id))
                .OrderBy(i => i.Id); // Ordenar por ID (fecha de creación)
            foreach (var it in iterations)
            {
                var iterationVm = new IterationItemViewModel
                {
                    Id = it.Id,
                    PhaseId = it.PhaseId,
                    Name = it.Name ?? "Sin nombre",
                    Goal = it.Goal,
                    StartDate = it.StartDate,
                    EndDate = it.EndDate,
                    CompletionPercentage = it.CompletionPercentage,
                    IsActive = it.IsActive
                };

                // Load microincrements for this iteration
                var microincrements = await microincrementService.GetMicroincrementsByIterationIdAsync(it.Id);
                foreach (var micro in microincrements)
                {
                    var authorName = "Desconocido";
                    if (micro.AuthorId.HasValue && userRepo != null)
                    {
                        var author = await userRepo.GetByIdAsync(micro.AuthorId.Value);
                        authorName = author?.Username ?? "Desconocido";
                    }

                    string? artifactName = null;
                    if (micro.ArtifactId.HasValue && artifactRepo != null)
                    {
                        var artifact = await artifactRepo.GetByIdAsync(micro.ArtifactId.Value);
                        artifactName = artifact?.Name;
                    }

                    iterationVm.Microincrements.Add(new MicroincrementItemViewModel
                    {
                        Id = micro.Id,
                        IterationId = micro.IterationId,
                        Title = micro.Title,
                        Description = micro.Description,
                        Type = micro.Type,
                        Date = micro.Date,
                        AuthorName = authorName,
                        ArtifactId = micro.ArtifactId,
                        ArtifactName = artifactName,
                        EvidenceUrl = micro.EvidenceUrl
                    });
                }

                vm.Iterations.Add(iterationVm);
            }
        }
        catch
        {
            // ignore for now - visual only
        }
    }

    private async System.Threading.Tasks.Task LoadIterationsForProjectAsync(ProjectViewModel vm)
    {
        // This method loads all iterations for all phases (used on initial load)
        await LoadIterationsForCurrentPhaseAsync(vm);
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
                    var newIterationVm = new IterationItemViewModel
                    {
                        Id = sr.Iteration.Id,
                        PhaseId = sr.Iteration.PhaseId,
                        Name = sr.Iteration.Name ?? "Sin nombre",
                        Goal = sr.Iteration.Goal,
                        StartDate = sr.Iteration.StartDate,
                        EndDate = sr.Iteration.EndDate,
                        CompletionPercentage = sr.Iteration.CompletionPercentage
                    };
                    projectVm.Iterations.Add(newIterationVm);
                }
            }
        }
    }

    private void OpenDashboardWindow()
    {
        var dashboardWindow = new DashboardWindow();
        if (VisualRoot is Window parentWindow)
        {
            dashboardWindow.ShowDialog(parentWindow);
        }
        else
        {
            dashboardWindow.Show();
        }
    }

    private async System.Threading.Tasks.Task LoadArtifactsForCurrentPhaseAsync(ProjectViewModel vm)
    {
        var artifactRepo = Program.ServiceProvider.GetService<IArtifactRepository>();
        var artifactVersionService = Program.ServiceProvider.GetService<IArtifactVersionService>();
        var phaseRepo = Program.ServiceProvider.GetService<IPhaseRepository>();
        
        if (artifactRepo == null || phaseRepo == null) return;

        try
        {
            var phases = (await phaseRepo.GetByProjectIdAsync(vm.ProjectId)).ToList();
            var currentPhase = phases.FirstOrDefault(p => p.Name == vm.CurrentPhaseName);
            
            if (currentPhase == null) return;

            var artifacts = await artifactRepo.GetByPhaseIdAsync(currentPhase.Id);
            vm.PhaseArtifacts.Clear();

            foreach (var artifact in artifacts)
            {
                // Get latest version
                var latestVersionResult = artifactVersionService != null 
                    ? await artifactVersionService.GetLatestVersionAsync(artifact.Id) 
                    : null;

                vm.PhaseArtifacts.Add(new ArtifactItemViewModel
                {
                    Id = artifact.Id,
                    ProjectId = artifact.ProjectId,
                    PhaseId = artifact.PhaseId,
                    Name = artifact.Name,
                    ArtifactType = artifact.ArtifactType,
                    Mandatory = artifact.Mandatory,
                    Description = artifact.Description,
                    CurrentState = artifact.CurrentState,
                    CurrentVersion = latestVersionResult?.VersionNumber ?? 0,
                    LastModified = latestVersionResult?.ArtifactVersion?.CreatedAt
                });
            }
        }
        catch
        {
            // ignore errors for now
        }
    }

    private async void OpenRegisterArtifactChangeWindow(int artifactId, string artifactName)
    {
        if (DataContext is not ProjectViewModel projectVm) return;

        var artifactVersionService = Program.ServiceProvider.GetService<IArtifactVersionService>();
        var microincrementService = Program.ServiceProvider.GetService<IMicroincrementService>();
        var iterationService = Program.ServiceProvider.GetService<IIterationService>();
        var phaseRepo = Program.ServiceProvider.GetService<IPhaseRepository>();
        
        if (artifactVersionService == null || microincrementService == null || 
            iterationService == null || phaseRepo == null) return;

        var vm = new RegisterArtifactViewModel(
            artifactVersionService, 
            microincrementService, 
            iterationService);

        // Get current phase ID
        var phases = (await phaseRepo.GetByProjectIdAsync(projectVm.ProjectId)).ToList();
        var currentPhase = phases.FirstOrDefault(p => p.Name == projectVm.CurrentPhaseName);
        
        if (currentPhase == null) return;

        // Pasar el ID del usuario actual
        await vm.InitializeAsync(artifactId, artifactName, projectVm.ProjectId, currentPhase.Id, projectVm.CurrentUserId);

        var window = new RegisterArtifactWindow(artifactVersionService, microincrementService);
        window.SetViewModel(vm);

        if (VisualRoot is Window parent)
        {
            var result = await window.ShowDialog<bool?>(parent);
            
            // If save was successful, reload data
            if (result == true)
            {
                await LoadIterationsForProjectAsync(projectVm);
                await LoadArtifactsForCurrentPhaseAsync(projectVm);
            }
        }
    }

    private async void OpenArtifactHistoryWindow(int artifactId, string artifactName)
    {
        var artifactVersionService = Program.ServiceProvider.GetService<IArtifactVersionService>();
        var previewService = Program.ServiceProvider.GetService<IArtifactPreviewService>();
        
        if (artifactVersionService == null) return;

        var vm = new ArtifactHistoryViewModel();
        vm.LoadVersionHistory(artifactId, artifactName);

        var window = new ArtifactHistoryWindow(artifactVersionService, previewService!);
        window.SetViewModel(vm);

        if (VisualRoot is Window parent)
        {
            await window.ShowDialog(parent);
        }
    }

    private async void PreviewArtifact(int artifactId)
    {
        var artifactVersionService = Program.ServiceProvider.GetService<IArtifactVersionService>();
        if (artifactVersionService == null) return;

        try
        {
            var result = await artifactVersionService.GetLatestVersionAsync(artifactId);
            if (!result.Success || result.ArtifactVersion == null)
            {
                // Show error message
                return;
            }

            var version = result.ArtifactVersion;

            // Check if it's a URL
            if (!string.IsNullOrEmpty(version.BuildInfo) &&
                (version.BuildInfo.StartsWith("http://", StringComparison.OrdinalIgnoreCase) ||
                 version.BuildInfo.StartsWith("https://", StringComparison.OrdinalIgnoreCase)))
            {
                // Open URL in browser
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                {
                    FileName = version.BuildInfo,
                    UseShellExecute = true
                });
                return;
            }

            // If it has a file, save and open it
            if (version.FileBlob != null && version.FileBlob.Length > 0)
            {
                var tempPath = System.IO.Path.GetTempPath();
                var extension = GetExtensionFromMime(version.FileMime);
                var fileName = $"artifact_latest_{DateTime.Now:yyyyMMddHHmmss}{extension}";
                var fullPath = System.IO.Path.Combine(tempPath, fileName);

                await System.IO.File.WriteAllBytesAsync(fullPath, version.FileBlob);

                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                {
                    FileName = fullPath,
                    UseShellExecute = true
                });
            }
        }
        catch
        {
            // Handle error
        }
    }

    private string GetExtensionFromMime(string? mimeType)
    {
        if (string.IsNullOrEmpty(mimeType)) return ".bin";

        return mimeType.ToLowerInvariant() switch
        {
            "application/pdf" => ".pdf",
            "application/msword" => ".doc",
            "application/vnd.openxmlformats-officedocument.wordprocessingml.document" => ".docx",
            "application/vnd.ms-excel" => ".xls",
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" => ".xlsx",
            "text/plain" => ".txt",
            "image/png" => ".png",
            "image/jpeg" => ".jpg",
            "image/gif" => ".gif",
            "application/zip" => ".zip",
            _ => ".bin"
        };
    }

    private async void OpenArtifactsWindow()
    {
        if (DataContext is not ProjectViewModel projectVm) return;

        var artifactService = Program.ServiceProvider.GetService<IArtifactService>();
        var artifactVersionService = Program.ServiceProvider.GetService<IArtifactVersionService>();
        var phaseRepo = Program.ServiceProvider.GetService<IPhaseRepository>();
        
        if (artifactService == null || artifactVersionService == null || phaseRepo == null) return;

        // Get the phase ID for the current phase name
        var phases = (await phaseRepo.GetByProjectIdAsync(projectVm.ProjectId)).ToList();
        var currentPhase = phases.FirstOrDefault(p => p.Name == projectVm.CurrentPhaseName);

        if (currentPhase == null)
        {
            // Fallback to first phase if not found
            currentPhase = phases.FirstOrDefault();
            if (currentPhase == null) return;
        }

        var vm = new ArtifactAdministrationViewModel();
        vm.Initialize(projectVm.ProjectId, currentPhase.Id, projectVm.CurrentPhaseName);

        // Subscribe to artifacts changed to reload in real-time
        vm.ArtifactsChanged += async () => await LoadArtifactsForCurrentPhaseAsync(projectVm);

        var window = new ArtifactAdministrationWindow(artifactService, artifactVersionService);
        window.SetViewModel(vm);

        if (VisualRoot is Window parent)
        {
            await window.ShowDialog<bool?>(parent);
            
            // Reload artifacts after closing (in case of any changes)
            await LoadArtifactsForCurrentPhaseAsync(projectVm);
        }
    }
}
