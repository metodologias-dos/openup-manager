using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Platform.Storage;
using HeyRed.Mime;
using OpenUpMan.Services;
using OpenUpMan.UI.ViewModels;

namespace OpenUpMan.UI.Views.Artifact;

public partial class RegisterArtifactWindow : Window
{
    private readonly IArtifactVersionService? _artifactVersionService;
    private readonly IMicroincrementService? _microincrementService;

    public RegisterArtifactWindow()
    {
        InitializeComponent();
    }

    public RegisterArtifactWindow(IArtifactVersionService artifactVersionService, IMicroincrementService microincrementService)
        : this()
    {
        _artifactVersionService = artifactVersionService;
        _microincrementService = microincrementService;
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public void SetViewModel(RegisterArtifactViewModel viewModel)
    {
        DataContext = viewModel;
        
        // Suscribirse a eventos del ViewModel
        viewModel.CloseRequested += () => Close();
        viewModel.SaveRequested += async () => await OnSaveAsync();
        viewModel.SelectFileRequested += async () => await OnSelectFile();
    }

    private async Task OnSelectFile()
    {
        var viewModel = DataContext as RegisterArtifactViewModel;
        if (viewModel == null) return;

        var files = await StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Seleccionar Archivo",
            AllowMultiple = false
        });

        if (files.Count > 0)
        {
            var file = files[0];
            var filePath = file.Path.LocalPath;
            var fileName = file.Name;
            
            viewModel.SetSelectedFile(filePath, fileName);
        }
    }

    private async Task OnSaveAsync()
    {
        var viewModel = DataContext as RegisterArtifactViewModel;
        if (viewModel == null || _artifactVersionService == null || _microincrementService == null)
        {
            await ShowErrorAsync("Error", "Servicios no disponibles.");
            return;
        }

        try
        {
            byte[]? fileBlob = null;
            string? fileMime = null;
            string? buildInfo = null;

            // Prepare data based on upload type
            if (viewModel.IsUploadingFile && !string.IsNullOrWhiteSpace(viewModel.FilePath))
            {
                // Read file
                fileBlob = await File.ReadAllBytesAsync(viewModel.FilePath);
                
                // Detect MIME type from file extension
                var extension = Path.GetExtension(viewModel.FilePath);
                fileMime = MimeTypesMap.GetMimeType(extension);
            }
            else if (!viewModel.IsUploadingFile && !string.IsNullOrWhiteSpace(viewModel.Url))
            {
                // Store URL in BuildInfo
                buildInfo = viewModel.Url;
            }

            // Create artifact version
            var versionResult = await _artifactVersionService.CreateVersionAsync(
                artifactId: viewModel.ArtifactId,
                createdBy: viewModel.CurrentUserId,
                notes: viewModel.Notes,
                fileBlob: fileBlob,
                fileMime: fileMime,
                buildInfo: buildInfo
            );

            if (!versionResult.Success)
            {
                await ShowErrorAsync("Error", versionResult.Message);
                return;
            }

            // Create microincrement
            var microTitle = $"{viewModel.ArtifactName} - v{versionResult.VersionNumber}";
            var microDescription = viewModel.Notes ?? $"Nueva versión del artefacto: {viewModel.ArtifactName}";
            var evidenceUrl = viewModel.IsUploadingFile ? null : viewModel.Url;

            var microResult = await _microincrementService.CreateMicroincrementAsync(
                iterationId: viewModel.ActiveIterationId!.Value,
                title: microTitle,
                description: microDescription,
                authorId: viewModel.CurrentUserId,
                type: "artifact",
                artifactId: viewModel.ArtifactId,
                evidenceUrl: evidenceUrl
            );

            // Si hubo error en el microincremento, solo loguearlo pero no mostrar diálogo
            if (!microResult.Success)
            {
                System.Diagnostics.Debug.WriteLine($"Warning: Microincrement creation failed: {microResult.Message}");
            }

            Close(true);
        }
        catch (Exception ex)
        {
            await ShowErrorAsync("Error", $"Error al guardar: {ex.Message}");
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
                    new TextBlock { Text = message },
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

    private async Task ShowWarningAsync(string title, string message)
    {
        await ShowErrorAsync(title, message);
    }
}

