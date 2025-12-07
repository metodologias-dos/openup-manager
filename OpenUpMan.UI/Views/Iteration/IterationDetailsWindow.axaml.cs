using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.DependencyInjection;
using OpenUpMan.Data;
using OpenUpMan.Services;
using OpenUpMan.UI.ViewModels;

namespace OpenUpMan.UI.Views;

public partial class IterationDetailsWindow : Window
{
    public IterationDetailsWindow()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    private void Close_Click(object? sender, RoutedEventArgs e)
    {
        Close();
    }

    private async void PreviewArtifact_Click(object? sender, RoutedEventArgs e)
    {
        if (sender is not Button button || button.Tag is not MicroincrementItemViewModel microincrement)
            return;

        if (!microincrement.ArtifactId.HasValue)
            return;

        var artifactVersionService = Program.ServiceProvider.GetService<IArtifactVersionService>();
        var artifactRepo = Program.ServiceProvider.GetService<OpenUpMan.Data.IArtifactRepository>();
        var projectRepo = Program.ServiceProvider.GetService<OpenUpMan.Data.IProjectRepository>();
        
        if (artifactVersionService == null)
            return;

        try
        {
            // Obtener la versión más reciente del artefacto
            var versionResult = await artifactVersionService.GetLatestVersionAsync(microincrement.ArtifactId.Value);
            
            if (versionResult?.ArtifactVersion == null)
            {
                await ShowErrorDialog("No se encontró una versión del artefacto.");
                return;
            }

            var version = versionResult.ArtifactVersion;

            // Obtener código del proyecto para crear carpeta organizada
            string projectCode = "unknown";
            if (artifactRepo != null && projectRepo != null)
            {
                var artifact = await artifactRepo.GetByIdAsync(microincrement.ArtifactId.Value);
                if (artifact != null)
                {
                    var project = await projectRepo.GetByIdAsync(artifact.ProjectId);
                    if (project != null)
                    {
                        projectCode = project.Name.Replace(" ", "_").ToLower();
                    }
                }
            }

            // Si tiene URL (BuildInfo), abrirla en el navegador
            if (!string.IsNullOrWhiteSpace(version.BuildInfo))
            {
                try
                {
                    var psi = new ProcessStartInfo
                    {
                        FileName = version.BuildInfo,
                        UseShellExecute = true
                    };
                    Process.Start(psi);
                }
                catch (Exception ex)
                {
                    await ShowErrorDialog($"No se pudo abrir el enlace: {ex.Message}");
                }
                return;
            }

            // Si tiene archivo, guardarlo en carpeta organizada y abrirlo
            if (version.FileBlob != null && version.FileBlob.Length > 0)
            {
                var extension = GetExtensionFromMime(version.FileMime);
                
                // Crear carpeta organizada: temp/openupman/{project_code}/
                var openUpManTempFolder = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "openupman", projectCode);
                System.IO.Directory.CreateDirectory(openUpManTempFolder);
                
                var fileName = $"artifact_{microincrement.ArtifactId}_{version.VersionNumber}{extension}";
                var tempPath = System.IO.Path.Combine(openUpManTempFolder, fileName);
                
                await System.IO.File.WriteAllBytesAsync(tempPath, version.FileBlob);

                try
                {
                    // Si es un archivo comprimido, abrir el explorador
                    if (IsCompressedFile(extension))
                    {
                        var psi = new ProcessStartInfo
                        {
                            FileName = "explorer.exe",
                            Arguments = $"/select,\"{tempPath}\"",
                            UseShellExecute = true
                        };
                        Process.Start(psi);
                    }
                    else
                    {
                        // Para otros archivos, abrirlos con la aplicación predeterminada
                        var psi = new ProcessStartInfo
                        {
                            FileName = tempPath,
                            UseShellExecute = true
                        };
                        Process.Start(psi);
                    }
                }
                catch (Exception ex)
                {
                    await ShowErrorDialog($"No se pudo abrir el archivo: {ex.Message}");
                }
                return;
            }

            await ShowErrorDialog("No hay contenido para previsualizar.");
        }
        catch (Exception ex)
        {
            await ShowErrorDialog($"Error al previsualizar: {ex.Message}");
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

    private async Task ShowErrorDialog(string message)
    {
        var dialog = new Window
        {
            Title = "Error",
            Width = 400,
            Height = 200,
            WindowStartupLocation = WindowStartupLocation.CenterOwner,
            Content = new StackPanel
            {
                Margin = new Avalonia.Thickness(20),
                Spacing = 12,
                Children =
                {
                    new TextBlock { Text = message, TextWrapping = Avalonia.Media.TextWrapping.Wrap },
                    new Button { Content = "Cerrar", HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Right }
                }
            }
        };

        var closeButton = (dialog.Content as StackPanel)?.Children.OfType<Button>().FirstOrDefault();
        if (closeButton != null)
        {
            closeButton.Click += (s, e) => dialog.Close();
        }

        await dialog.ShowDialog(this);
    }
}

