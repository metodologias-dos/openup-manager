using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.DependencyInjection;
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

            // Si tiene URL, abrirla en el navegador
            if (!string.IsNullOrEmpty(version.BuildInfo) && Uri.TryCreate(version.BuildInfo, UriKind.Absolute, out var uri))
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = version.BuildInfo,
                    UseShellExecute = true
                });
                return;
            }

            // Si tiene archivo, guardarlo temporalmente y abrirlo
            if (version.FileBlob != null && version.FileBlob.Length > 0)
            {
                var tempPath = System.IO.Path.Combine(
                    System.IO.Path.GetTempPath(),
                    $"artifact_{version.Id}_{DateTime.Now:yyyyMMddHHmmss}{GetFileExtension(version.FileMime)}"
                );

                await System.IO.File.WriteAllBytesAsync(tempPath, version.FileBlob);

                Process.Start(new ProcessStartInfo
                {
                    FileName = tempPath,
                    UseShellExecute = true
                });
                return;
            }

            await ShowErrorDialog("El artefacto no tiene archivo ni URL asociado.");
        }
        catch (Exception ex)
        {
            await ShowErrorDialog($"Error al previsualizar: {ex.Message}");
        }
    }

    private string GetFileExtension(string? mimeType)
    {
        return mimeType switch
        {
            "application/pdf" => ".pdf",
            "application/msword" => ".doc",
            "application/vnd.openxmlformats-officedocument.wordprocessingml.document" => ".docx",
            "text/plain" => ".txt",
            "image/png" => ".png",
            "image/jpeg" => ".jpg",
            "application/zip" => ".zip",
            _ => ".bin"
        };
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

