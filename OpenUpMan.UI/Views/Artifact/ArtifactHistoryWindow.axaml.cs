using System;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Platform.Storage;
using Microsoft.Extensions.DependencyInjection;
using OpenUpMan.Data;
using OpenUpMan.Services;
using OpenUpMan.UI.ViewModels;

namespace OpenUpMan.UI.Views.Artifact;

public partial class ArtifactHistoryWindow : Window
{
    private readonly IArtifactVersionService? _artifactVersionService;
    private readonly IArtifactPreviewService? _previewService;

    public ArtifactHistoryWindow()
    {
        InitializeComponent();
    }

    public ArtifactHistoryWindow(IArtifactVersionService artifactVersionService, IArtifactPreviewService previewService)
        : this()
    {
        _artifactVersionService = artifactVersionService;
        _previewService = previewService;
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public void SetViewModel(ArtifactHistoryViewModel viewModel)
    {
        DataContext = viewModel;
        
        // Suscribirse a eventos del ViewModel
        viewModel.CloseRequested += () => Close();
        viewModel.PreviewVersionRequested += async (versionId) => await OnPreviewVersionAsync(versionId);
        
        // Cargar versiones
        LoadVersionsAsync(viewModel).ConfigureAwait(false);
    }

    private async Task LoadVersionsAsync(ArtifactHistoryViewModel viewModel)
    {
        if (_artifactVersionService == null)
            return;

        try
        {
            var userRepo = Program.ServiceProvider.GetService<IUserRepository>();
            
            if (userRepo != null)
            {
                await viewModel.LoadVersionHistoryAsync(_artifactVersionService, userRepo);
            }
        }
        catch (Exception ex)
        {
            await ShowErrorAsync("Error", $"Error al cargar historial: {ex.Message}");
        }
    }

    private async Task OnPreviewVersionAsync(int versionId)
    {
        if (_artifactVersionService == null)
        {
            await ShowErrorAsync("Error", "Servicio no disponible.");
            return;
        }

        try
        {
            var viewModel = DataContext as ArtifactHistoryViewModel;
            if (viewModel == null) return;

            var versionVm = viewModel.Versions.FirstOrDefault(v => v.Id == versionId);
            if (versionVm == null) return;

            var versionResult = await _artifactVersionService.GetVersionAsync(versionVm.ArtifactId, versionVm.VersionNumber);
            if (!versionResult.Success || versionResult.ArtifactVersion == null)
            {
                await ShowErrorAsync("Error", "No se pudo obtener la versión del artefacto.");
                return;
            }

            var version = versionResult.ArtifactVersion;

            // Si tiene BuildInfo (URL), abrirlo en el navegador
            if (!string.IsNullOrWhiteSpace(version.BuildInfo))
            {
                try
                {
                    var psi = new System.Diagnostics.ProcessStartInfo
                    {
                        FileName = version.BuildInfo,
                        UseShellExecute = true
                    };
                    System.Diagnostics.Process.Start(psi);
                }
                catch (Exception ex)
                {
                    await ShowErrorAsync("Error", $"No se pudo abrir el enlace: {ex.Message}");
                }
                return;
            }

            // Si tiene archivo, guardarlo en carpeta organizada y abrirlo
            if (version.FileBlob != null && version.FileBlob.Length > 0)
            {
                var extension = GetExtensionFromMime(version.FileMime);
                
                // Obtener código del proyecto para crear carpeta organizada
                string projectCode = "unknown";
                var artifactRepo = Program.ServiceProvider.GetService<IArtifactRepository>();
                var projectRepo = Program.ServiceProvider.GetService<IProjectRepository>();
                
                if (artifactRepo != null && projectRepo != null)
                {
                    var artifact = await artifactRepo.GetByIdAsync(versionVm.ArtifactId);
                    if (artifact != null)
                    {
                        var project = await projectRepo.GetByIdAsync(artifact.ProjectId);
                        if (project != null)
                        {
                            projectCode = project.Name.Replace(" ", "_").ToLower();
                        }
                    }
                }
                
                // Crear carpeta organizada: temp/openupman/{project_code}/
                var openUpManTempFolder = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "openupman", projectCode);
                System.IO.Directory.CreateDirectory(openUpManTempFolder);
                
                var fileName = $"{viewModel.ArtifactName}_v{version.VersionNumber}{extension}".Replace(" ", "_");
                var tempPath = System.IO.Path.Combine(openUpManTempFolder, fileName);
                
                await System.IO.File.WriteAllBytesAsync(tempPath, version.FileBlob);

                try
                {
                    // Si es un archivo comprimido, abrir el explorador de archivos
                    if (IsCompressedFile(extension))
                    {
                        var psi = new System.Diagnostics.ProcessStartInfo
                        {
                            FileName = "explorer.exe",
                            Arguments = $"/select,\"{tempPath}\"",
                            UseShellExecute = true
                        };
                        System.Diagnostics.Process.Start(psi);
                    }
                    else
                    {
                        // Para otros archivos, abrirlos con la aplicación predeterminada
                        var psi = new System.Diagnostics.ProcessStartInfo
                        {
                            FileName = tempPath,
                            UseShellExecute = true
                        };
                        System.Diagnostics.Process.Start(psi);
                    }
                }
                catch (Exception ex)
                {
                    await ShowErrorAsync("Error", $"No se pudo abrir el archivo: {ex.Message}");
                }
                return;
            }

            await ShowErrorAsync("Error", "No hay contenido para previsualizar.");
        }
        catch (Exception ex)
        {
            await ShowErrorAsync("Error", $"Error al previsualizar: {ex.Message}");
        }
    }

    private string GetExtensionFromMime(string? mimeType)
    {
        if (string.IsNullOrWhiteSpace(mimeType))
            return ".bin";

        return mimeType switch
        {
            "application/pdf" => ".pdf",
            "image/png" => ".png",
            "image/jpeg" => ".jpg",
            "image/gif" => ".gif",
            "image/bmp" => ".bmp",
            "image/svg+xml" => ".svg",
            "text/plain" => ".txt",
            "text/html" => ".html",
            "application/msword" => ".doc",
            "application/vnd.openxmlformats-officedocument.wordprocessingml.document" => ".docx",
            "application/vnd.ms-excel" => ".xls",
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" => ".xlsx",
            "application/zip" => ".zip",
            "application/x-rar-compressed" => ".rar",
            "application/x-7z-compressed" => ".7z",
            _ => ".bin"
        };
    }

    private bool IsCompressedFile(string extension)
    {
        var compressedExtensions = new[] { ".zip", ".rar", ".7z", ".tar", ".gz", ".bz2" };
        return compressedExtensions.Contains(extension.ToLowerInvariant());
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
}

