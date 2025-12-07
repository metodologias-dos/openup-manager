using System;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using OpenUpMan.Services;
using OpenUpMan.UI.ViewModels;

namespace OpenUpMan.UI.Views.Artifact;

public partial class ArtifactAdministrationWindow : Window
{
    private readonly IArtifactService? _artifactService;
    private readonly IArtifactVersionService? _artifactVersionService;

    public ArtifactAdministrationWindow()
    {
        InitializeComponent();
    }

    public ArtifactAdministrationWindow(IArtifactService artifactService, IArtifactVersionService artifactVersionService)
        : this()
    {
        _artifactService = artifactService;
        _artifactVersionService = artifactVersionService;
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public void SetViewModel(ArtifactAdministrationViewModel viewModel)
    {
        DataContext = viewModel;
        
        // Suscribirse a eventos del ViewModel
        viewModel.CloseRequested += () => Close();
        viewModel.PhaseChanged += async (phaseId) => await LoadArtifactsForPhaseAsync(phaseId);
        viewModel.ArtifactsChanged += async () => await SaveCurrentArtifactAsync();
        viewModel.DeleteArtifactRequested += async (artifactId) => await DeleteArtifactAsync(artifactId);
        viewModel.CreateArtifactRequested += async () => await OpenCreateArtifactDialogAsync();
        viewModel.EditArtifactRequested += async (artifact) => await OpenEditArtifactDialogAsync(artifact);
        
        // Cargar artefactos iniciales
        LoadArtifactsForPhaseAsync(viewModel.SelectedPhaseId).ConfigureAwait(false);
    }

    private async Task LoadArtifactsForPhaseAsync(int phaseId)
    {
        if (_artifactService == null || _artifactVersionService == null)
            return;

        var viewModel = DataContext as ArtifactAdministrationViewModel;
        if (viewModel == null) return;

        try
        {
            var artifacts = await _artifactService.GetArtifactsByPhaseIdAsync(phaseId);
            
            viewModel.Artifacts.Clear();
            foreach (var artifact in artifacts)
            {
                // Get latest version info
                var latestVersion = await _artifactVersionService.GetLatestVersionAsync(artifact.Id);
                
                var artifactVm = new ArtifactItemViewModel
                {
                    Id = artifact.Id,
                    ProjectId = artifact.ProjectId,
                    PhaseId = artifact.PhaseId,
                    Name = artifact.Name,
                    ArtifactType = artifact.ArtifactType,
                    Mandatory = artifact.Mandatory,
                    Description = artifact.Description,
                    CurrentState = artifact.CurrentState,
                    CurrentVersion = latestVersion.ArtifactVersion?.VersionNumber ?? 0,
                    LastUpdated = latestVersion.ArtifactVersion?.CreatedAt,
                    LastUpdatedBy = latestVersion.ArtifactVersion?.CreatedBy?.ToString() // TODO: Get actual user name
                };
                
                viewModel.Artifacts.Add(artifactVm);
            }
        }
        catch (Exception ex)
        {
            await ShowErrorAsync("Error", $"Error al cargar artefactos: {ex.Message}");
        }
    }

    private async Task DeleteArtifactAsync(int artifactId)
    {
        if (_artifactService == null)
            return;

        var viewModel = DataContext as ArtifactAdministrationViewModel;
        if (viewModel == null) return;

        try
        {
            var result = await _artifactService.DeleteArtifactAsync(artifactId);

            if (result.Success)
            {
                // Remove from ViewModel collection
                var artifactToRemove = viewModel.Artifacts.FirstOrDefault(a => a.Id == artifactId);
                if (artifactToRemove != null)
                {
                    viewModel.Artifacts.Remove(artifactToRemove);
                }
                
                // Notify that artifacts have changed to update ProjectView
                viewModel.NotifyArtifactsChanged();
            }
            else
            {
                await ShowErrorAsync("Error", result.Message);
            }
        }
        catch (Exception ex)
        {
            await ShowErrorAsync("Error", $"Error al eliminar artefacto: {ex.Message}");
        }
    }

    private async Task OpenCreateArtifactDialogAsync()
    {
        if (_artifactService == null)
            return;

        var viewModel = DataContext as ArtifactAdministrationViewModel;
        if (viewModel == null) return;

        var editorVm = new ArtifactEditorViewModel();
        editorVm.InitializeForNew();

        var dialog = new ArtifactEditorWindow();
        dialog.SetViewModel(editorVm);

        var result = await dialog.ShowDialog<bool?>(this);

        if (result == true && !string.IsNullOrWhiteSpace(editorVm.Name))
        {
            try
            {
                var createResult = await _artifactService.CreateArtifactAsync(
                    viewModel.ProjectId,
                    viewModel.SelectedPhaseId,
                    editorVm.Name,
                    editorVm.ArtifactType,
                    editorVm.IsMandatory,
                    editorVm.Description
                );

                if (createResult.Success)
                {
                    await LoadArtifactsForPhaseAsync(viewModel.SelectedPhaseId);
                    viewModel.NotifyArtifactsChanged();
                }
                else
                {
                    await ShowErrorAsync("Error", createResult.Message);
                }
            }
            catch (Exception ex)
            {
                await ShowErrorAsync("Error", $"Error al crear artefacto: {ex.Message}");
            }
        }
    }

    private async Task OpenEditArtifactDialogAsync(ArtifactItemViewModel artifact)
    {
        if (_artifactService == null)
            return;

        var viewModel = DataContext as ArtifactAdministrationViewModel;
        if (viewModel == null) return;

        var editorVm = new ArtifactEditorViewModel();
        editorVm.InitializeForEdit(
            artifact.Id,
            artifact.Name,
            artifact.ArtifactType,
            artifact.Mandatory,
            artifact.Description
        );

        var dialog = new ArtifactEditorWindow();
        dialog.SetViewModel(editorVm);

        var result = await dialog.ShowDialog<bool?>(this);

        if (result == true && !string.IsNullOrWhiteSpace(editorVm.Name))
        {
            try
            {
                var updateResult = await _artifactService.UpdateArtifactAsync(
                    artifact.Id,
                    editorVm.Name,
                    editorVm.ArtifactType,
                    editorVm.IsMandatory,
                    editorVm.Description
                );

                if (updateResult.Success)
                {
                    await LoadArtifactsForPhaseAsync(viewModel.SelectedPhaseId);
                    viewModel.NotifyArtifactsChanged();
                }
                else
                {
                    await ShowErrorAsync("Error", updateResult.Message);
                }
            }
            catch (Exception ex)
            {
                await ShowErrorAsync("Error", $"Error al actualizar artefacto: {ex.Message}");
            }
        }
    }

    private async Task SaveCurrentArtifactAsync()
    {
        if (_artifactService == null)
            return;

        var viewModel = DataContext as ArtifactAdministrationViewModel;
        if (viewModel == null) return;

        try
        {
            if (viewModel.SelectedArtifact != null)
            {
                // Update existing artifact
                var result = await _artifactService.UpdateArtifactAsync(
                    viewModel.SelectedArtifact.Id,
                    viewModel.EditName,
                    viewModel.EditArtifactType,
                    viewModel.EditMandatory,
                    viewModel.EditDescription
                );

                if (!result.Success)
                {
                    await ShowErrorAsync("Error", result.Message);
                    return;
                }

                // Artifact updated successfully, reload
                await LoadArtifactsForPhaseAsync(viewModel.SelectedPhaseId);
            }
            else if (viewModel.IsEditMode && !string.IsNullOrWhiteSpace(viewModel.EditName))
            {
                // Create new artifact
                var result = await _artifactService.CreateArtifactAsync(
                    viewModel.ProjectId,
                    viewModel.SelectedPhaseId,
                    viewModel.EditName,
                    viewModel.EditArtifactType,
                    viewModel.EditMandatory,
                    viewModel.EditDescription
                );

                if (!result.Success)
                {
                    await ShowErrorAsync("Error", result.Message);
                    return;
                }

                // Artifact created successfully, reload
                await LoadArtifactsForPhaseAsync(viewModel.SelectedPhaseId);
            }
        }
        catch (Exception ex)
        {
            await ShowErrorAsync("Error", $"Error al guardar artefacto: {ex.Message}");
        }
    }


    private async Task ShowErrorAsync(string title, string message)
    {
        var dialog = new Window
        {
            Title = title,
            Width = 400,
            Height = 150,
            Content = new StackPanel
            {
                Margin = new Avalonia.Thickness(20),
                Spacing = 10,
                Children =
                {
                    new TextBlock { Text = message, TextWrapping = Avalonia.Media.TextWrapping.Wrap },
                    new Button 
                    { 
                        Content = "OK", 
                        HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center 
                    }
                }
            }
        };

        await dialog.ShowDialog(this);
    }

    private async Task ShowInfoAsync(string title, string message)
    {
        await ShowErrorAsync(title, message);
    }

    private async Task<bool> ConfirmAsync(string title, string message)
    {
        bool result = false;
        
        var yesButton = new Button { Content = "Sí", MinWidth = 80 };
        var noButton = new Button { Content = "No", MinWidth = 80 };
        
        var dialog = new Window
        {
            Title = title,
            Width = 400,
            Height = 150,
            Content = new StackPanel
            {
                Margin = new Avalonia.Thickness(20),
                Spacing = 15,
                Children =
                {
                    new TextBlock { Text = message, TextWrapping = Avalonia.Media.TextWrapping.Wrap },
                    new StackPanel
                    {
                        Orientation = Avalonia.Layout.Orientation.Horizontal,
                        HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center,
                        Spacing = 10,
                        Children = { yesButton, noButton }
                    }
                }
            }
        };

        yesButton.Click += (_, _) => { result = true; dialog.Close(); };
        noButton.Click += (_, _) => { result = false; dialog.Close(); };

        await dialog.ShowDialog(this);
        return result;
    }
}

