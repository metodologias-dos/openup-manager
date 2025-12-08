using System;
using System.Collections.ObjectModel;
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

    private void OpenManageUsersDialog(ProjectUsersManagementViewModel vm)
    {
        var dialog = new ProjectUsersManagementDialog
        {
            DataContext = vm
        };

        if (VisualRoot is Window window)
        {
            dialog.ShowDialog(window);
        }
    }

    private void OnDataContextChanged(object? sender, EventArgs e)
    {
        if (DataContext is ProjectViewModel vm)
        {
            // Avoid double subscription if DataContext is set multiple times
            vm.SaveRequested -= OnSaveProject;
            vm.SaveRequested += OnSaveProject;
            
            vm.ManageArtifactsRequested -= OpenArtifactsWindow;
            vm.ManageArtifactsRequested += OpenArtifactsWindow;

            // Subscribe to create iteration requests
            vm.CreateIterationRequested -= OpenCreateIterationDialog;
            vm.CreateIterationRequested += OpenCreateIterationDialog;

            // Subscribe to edit iteration requests
            vm.EditIterationRequested -= OpenEditIterationDialog;
            vm.EditIterationRequested += OpenEditIterationDialog;

            // Subscribe to delete iteration requests
            vm.DeleteIterationRequested -= DeleteIteration;
            vm.DeleteIterationRequested += DeleteIteration;

            vm.OpenDashboardRequested -= OpenDashboardWindow;
            vm.OpenDashboardRequested += OpenDashboardWindow;

            // Subscribe to artifact-related events
            vm.ArtifactChangeRequested -= OpenRegisterArtifactChangeWindow;
            vm.ArtifactChangeRequested += OpenRegisterArtifactChangeWindow;

            vm.ArtifactHistoryRequested -= OpenArtifactHistoryWindow;
            vm.ArtifactHistoryRequested += OpenArtifactHistoryWindow;

            vm.ArtifactPreviewRequested -= PreviewArtifact;
            vm.ArtifactPreviewRequested += PreviewArtifact;

            vm.ManageUsersRequested -= OpenManageUsersDialog;
            vm.ManageUsersRequested += OpenManageUsersDialog;

            // Subscribe to activate iteration requests
            vm.ActivateIterationRequested -= ActivateIteration;
            vm.ActivateIterationRequested += ActivateIteration;

            // Subscribe to view iteration details requests
            vm.ViewIterationDetailsRequested -= OpenIterationDetailsWindow;
            vm.ViewIterationDetailsRequested += OpenIterationDetailsWindow;

            // Subscribe to phase change events
            vm.PhaseChanged -= OnPhaseChanged;
            vm.PhaseChanged += OnPhaseChanged;

            // Subscribe to start and end phase events
            vm.StartPhaseRequested -= OnStartPhase;
            vm.StartPhaseRequested += OnStartPhase;

            vm.EndPhaseRequested -= OnEndPhase;
            vm.EndPhaseRequested += OnEndPhase;

            // Load existing iterations for the project
            _ = LoadIterationsForProjectAsync(vm);

            // Load artifacts for current phase
            _ = LoadArtifactsForCurrentPhaseAsync(vm);
            
            // Load phase status
            _ = LoadPhaseStatusAsync(vm);
        }
    }

    private async void OnPhaseChanged()
    {
        if (DataContext is ProjectViewModel vm)
        {
            await LoadIterationsForCurrentPhaseAsync(vm);
            await LoadArtifactsForCurrentPhaseAsync(vm);
            await LoadPhaseStatusAsync(vm);
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
                .OrderBy(i => i.Id); // Ordenar por ID (fecha de creaciï¿½n)
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
                    IsActive = it.IsActive,
                    Microincrements = new ObservableCollection<MicroincrementItemViewModel>()
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

        // Get current phase
        var currentPhase = phases.FirstOrDefault(p => p.Name == projectVm.CurrentPhaseName);
        if (currentPhase == null)
        {
            currentPhase = phases.First();
        }

        var dialog = new IterationCreateWindow();
        dialog.SetPhase(currentPhase.Id, currentPhase.Name);

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
                        EndDate = sr.Iteration.EndDate
                    };
                    projectVm.Iterations.Add(newIterationVm);
                }
            }
        }
    }

    private async void OpenEditIterationDialog(IterationItemViewModel iteration)
    {
        if (DataContext is not ProjectViewModel projectVm) return;

        var iterationService = Program.ServiceProvider.GetService<IIterationService>();
        if (iterationService == null) return;

        var dialog = new IterationEditWindow();
        dialog.SetIteration(iteration, projectVm.CurrentPhaseName);

        if (VisualRoot is Window parent)
        {
            var result = await dialog.ShowDialog<object?>(parent);
            if (result != null)
            {
                // Extract data from result
                var resultType = result.GetType();
                var iterationId = (int)resultType.GetProperty("IterationId")?.GetValue(result)!;
                var name = (string)resultType.GetProperty("Name")?.GetValue(result)!;
                var goal = (string)resultType.GetProperty("Goal")?.GetValue(result)!;
                var startDate = (DateTime?)resultType.GetProperty("StartDate")?.GetValue(result);
                var endDate = (DateTime?)resultType.GetProperty("EndDate")?.GetValue(result);

                // Update using service
                var updateResult = await iterationService.UpdateIterationAsync(
                    iterationId, 
                    name, 
                    goal, 
                    startDate, 
                    endDate
                );

                if (updateResult.Success && updateResult.Iteration != null)
                {
                    // Update the ViewModel
                    iteration.Name = updateResult.Iteration.Name ?? "Sin nombre";
                    iteration.Goal = updateResult.Iteration.Goal;
                    iteration.StartDate = updateResult.Iteration.StartDate;
                    iteration.EndDate = updateResult.Iteration.EndDate;
                }
            }
        }
    }

    private async void DeleteIteration(IterationItemViewModel iteration)
    {
        if (DataContext is not ProjectViewModel projectVm) return;

        var iterationService = Program.ServiceProvider.GetService<IIterationService>();
        if (iterationService == null) return;

        // Verificar que no tenga microincrementos
        if (iteration.HasMicroincrements)
        {
            // No debería llegar aquí por la visibilidad del botón, pero por seguridad
            return;
        }

        // Confirmar eliminación
        if (VisualRoot is Window parent)
        {
            bool? result = null;
            
            var confirmWindow = new Window
            {
                Title = "Confirmar Eliminación",
                Width = 400,
                Height = 180,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                CanResize = false
            };

            var mainPanel = new StackPanel
            {
                Margin = new Avalonia.Thickness(20),
                Spacing = 15
            };

            mainPanel.Children.Add(new TextBlock
            {
                Text = "¿Está seguro que desea eliminar esta iteración?",
                FontSize = 14,
                TextWrapping = Avalonia.Media.TextWrapping.Wrap
            });

            mainPanel.Children.Add(new TextBlock
            {
                Text = iteration.Name,
                FontSize = 16,
                FontWeight = Avalonia.Media.FontWeight.Bold,
                Margin = new Avalonia.Thickness(0, 5, 0, 5)
            });

            var buttonPanel = new StackPanel
            {
                Orientation = Avalonia.Layout.Orientation.Horizontal,
                HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Right,
                Spacing = 10,
                Margin = new Avalonia.Thickness(0, 20, 0, 0)
            };

            var cancelButton = new Button
            {
                Content = "Cancelar",
                Width = 100,
                Height = 32
            };
            cancelButton.Click += (s, e) => { result = false; confirmWindow.Close(); };

            var confirmButton = new Button
            {
                Content = "Eliminar",
                Width = 100,
                Height = 32
            };
            confirmButton.Click += (s, e) => { result = true; confirmWindow.Close(); };

            buttonPanel.Children.Add(cancelButton);
            buttonPanel.Children.Add(confirmButton);
            mainPanel.Children.Add(buttonPanel);

            confirmWindow.Content = mainPanel;

            await confirmWindow.ShowDialog(parent);

            if (result != true)
                return;
        }

        try
        {
            var deleteResult = await iterationService.DeleteIterationAsync(iteration.Id);
            if (deleteResult.Success)
            {
                projectVm.Iterations.Remove(iteration);
            }
        }
        catch
        {
            // Handle error
        }
    }

    private void OpenDashboardWindow()
    {
        if (DataContext is not ProjectViewModel vm) return;

        var dashboardService = Program.ServiceProvider.GetService<IDashboardService>();
        if (dashboardService == null) return;

        var dashboardWindow = new DashboardWindow(dashboardService, vm.ProjectId);
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

    private async void OnStartPhase()
    {
        if (DataContext is not ProjectViewModel vm) return;

        var phaseService = Program.ServiceProvider.GetService<IPhaseService>();
        var phaseRepo = Program.ServiceProvider.GetService<IPhaseRepository>();
        
        if (phaseService == null || phaseRepo == null) return;

        try
        {
            // Get current phase
            var phases = (await phaseRepo.GetByProjectIdAsync(vm.ProjectId)).ToList();
            var currentPhase = phases.FirstOrDefault(p => p.Name == vm.CurrentPhaseName);
            
            if (currentPhase == null) return;

            // Call service to start phase
            var result = await phaseService.StartPhaseAsync(currentPhase.Id, vm.ProjectId);

            if (result.Success)
            {
                // Update ViewModel with new status and dates
                vm.UpdatePhaseStatus(result.Phase?.Status ?? "IN_PROGRESS");
                vm.UpdatePhaseDates(result.Phase?.StartDate, result.Phase?.EndDate);
                
                // Show success message
                await ShowMessageBox("Ã‰xito", result.Message, MessageBoxType.Success);
            }
            else
            {
                // Show warning or error message
                var messageType = result.ResultType == ServiceResultType.Warning 
                    ? MessageBoxType.Warning 
                    : MessageBoxType.Error;
                await ShowMessageBox("Advertencia", result.Message, messageType);
            }
        }
        catch (Exception ex)
        {
            await ShowMessageBox("Error", $"Error al iniciar la fase: {ex.Message}", MessageBoxType.Error);
        }
    }

    private async void OnEndPhase()
    {
        if (DataContext is not ProjectViewModel vm) return;

        var phaseService = Program.ServiceProvider.GetService<IPhaseService>();
        var phaseRepo = Program.ServiceProvider.GetService<IPhaseRepository>();
        
        if (phaseService == null || phaseRepo == null) return;

        try
        {
            // Get current phase
            var phases = (await phaseRepo.GetByProjectIdAsync(vm.ProjectId)).ToList();
            var currentPhase = phases.FirstOrDefault(p => p.Name == vm.CurrentPhaseName);
            
            if (currentPhase == null) return;

            // Call service to end phase
            var result = await phaseService.EndPhaseAsync(currentPhase.Id);

            if (result.Success)
            {
                // Update ViewModel with new status and dates
                vm.UpdatePhaseStatus(result.Phase?.Status ?? "DONE");
                vm.UpdatePhaseDates(result.Phase?.StartDate, result.Phase?.EndDate);
                
                // Show success message
                await ShowMessageBox("Ã‰xito", result.Message, MessageBoxType.Success);
            }
            else
            {
                // Show warning or error message
                var messageType = result.ResultType == ServiceResultType.Warning 
                    ? MessageBoxType.Warning 
                    : MessageBoxType.Error;
                await ShowMessageBox("Advertencia", result.Message, messageType);
            }
        }
        catch (Exception ex)
        {
            await ShowMessageBox("Error", $"Error al finalizar la fase: {ex.Message}", MessageBoxType.Error);
        }
    }

    private async System.Threading.Tasks.Task ShowMessageBox(string title, string message, MessageBoxType type)
    {
        if (VisualRoot is not Window parent) return;

        var messageBoxWindow = new MessageBoxWindow(title, message, type);
        await messageBoxWindow.ShowDialog(parent);
    }

    private async System.Threading.Tasks.Task LoadPhaseStatusAsync(ProjectViewModel vm)
    {
        var phaseRepo = Program.ServiceProvider.GetService<IPhaseRepository>();
        if (phaseRepo == null) return;

        try
        {
            var phases = (await phaseRepo.GetByProjectIdAsync(vm.ProjectId)).ToList();
            var currentPhase = phases.FirstOrDefault(p => p.Name == vm.CurrentPhaseName);
            
            if (currentPhase != null)
            {
                vm.CurrentPhaseId = currentPhase.Id;
                vm.UpdatePhaseStatus(currentPhase.Status);
                vm.UpdatePhaseDates(currentPhase.StartDate, currentPhase.EndDate);
                vm.UpdatePhaseTextFields(currentPhase.Objective, currentPhase.Scope, currentPhase.Observations);
            }
        }
        catch
        {
            // Ignore errors for now
        }
    }

    private async void OnSaveProject()
    {
        if (DataContext is not ProjectViewModel vm) return;

        var phaseService = Program.ServiceProvider.GetService<IPhaseService>();
        var projectRepo = Program.ServiceProvider.GetService<IProjectRepository>();
        
        if (phaseService == null || projectRepo == null) return;

        try
        {
            // Guardar nombre del proyecto
            var project = await projectRepo.GetByIdAsync(vm.ProjectId);
            if (project != null && project.Name != vm.ProjectName)
            {
                project.UpdateDetails(vm.ProjectName, project.Description, project.StartDate, project.Code);
                await projectRepo.UpdateAsync(project);
            }

            // Guardar campos de texto de la fase actual (Objetivo, Alcance, Observaciones)
            if (vm.CurrentPhaseId > 0)
            {
                // Actualizar objetivo
                var objectiveResult = await phaseService.UpdatePhaseObjectiveAsync(
                    vm.CurrentPhaseId, 
                    vm.CurrentPhaseObjective);

                // Actualizar alcance
                var scopeResult = await phaseService.UpdatePhaseScopeAsync(
                    vm.CurrentPhaseId, 
                    vm.CurrentPhaseScope);

                // Actualizar observaciones
                var observationsResult = await phaseService.UpdatePhaseObservationsAsync(
                    vm.CurrentPhaseId, 
                    vm.CurrentPhaseObservations);

                // Verificar si hubo algÃºn error
                if (!objectiveResult.Success || !scopeResult.Success || !observationsResult.Success)
                {
                    await ShowMessageBox("Advertencia", 
                        "Algunos campos no se pudieron guardar correctamente.", 
                        MessageBoxType.Warning);
                    return;
                }

                // Recargar los datos desde la BD para confirmar que se guardaron correctamente
                await LoadPhaseStatusAsync(vm);

                await ShowMessageBox("Ã‰xito", 
                    "Los cambios se han guardado correctamente.", 
                    MessageBoxType.Success);
            }
        }
        catch (Exception ex)
        {
            await ShowMessageBox("Error", 
                $"Error al guardar los cambios: {ex.Message}", 
                MessageBoxType.Error);
        }
    }
}
