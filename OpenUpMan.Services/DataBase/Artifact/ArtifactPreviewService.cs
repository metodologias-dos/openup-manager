using System.Diagnostics;
using System.IO;
using Microsoft.Extensions.Logging;
using OpenUpMan.Domain;

namespace OpenUpMan.Services;

/// <summary>
/// Service for handling artifact preview and file operations
/// </summary>
public class ArtifactPreviewService : IArtifactPreviewService
{
    private readonly ILogger<ArtifactPreviewService> _logger;
    private readonly string _tempDirectory;

    public ArtifactPreviewService(ILogger<ArtifactPreviewService> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _tempDirectory = Path.Combine(Path.GetTempPath(), "OpenUpMan", "Artifacts");
        
        // Ensure temp directory exists
        if (!Directory.Exists(_tempDirectory))
        {
            Directory.CreateDirectory(_tempDirectory);
        }
    }

    public async Task<PreviewResult> PreviewArtifactVersionAsync(ArtifactVersion version, string artifactName, CancellationToken ct = default)
    {
        try
        {
            // If it's a URL (no file blob)
            if (version.FileBlob == null || version.FileBlob.Length == 0)
            {
                // Try to get URL from BuildInfo or Notes
                var url = version.BuildInfo ?? version.Notes;
                if (!string.IsNullOrWhiteSpace(url) && (url.StartsWith("http://") || url.StartsWith("https://")))
                {
                    return await OpenUrlAsync(url);
                }
                
                return new PreviewResult(
                    Success: false,
                    Message: "No hay archivo o URL disponible para previsualizar."
                );
            }

            // Handle file blob
            var extension = GetExtensionFromMime(version.FileMime);
            var fileName = $"{artifactName}_v{version.VersionNumber}{extension}";
            var filePath = Path.Combine(_tempDirectory, fileName);

            // Write file to temp directory
            await File.WriteAllBytesAsync(filePath, version.FileBlob, ct);

            // Determine if file can be opened directly or just show in folder
            if (CanOpenDirectly(version.FileMime))
            {
                return await OpenFileAsync(filePath);
            }
            else
            {
                return await OpenInExplorerAsync(filePath);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error previewing artifact version {VersionId}", version.Id);
            return new PreviewResult(
                Success: false,
                Message: $"Error al previsualizar: {ex.Message}"
            );
        }
    }

    public async Task<DownloadResult> DownloadArtifactVersionAsync(ArtifactVersion version, string artifactName, string targetDirectory, CancellationToken ct = default)
    {
        try
        {
            if (version.FileBlob == null || version.FileBlob.Length == 0)
            {
                return new DownloadResult(
                    Success: false,
                    Message: "No hay archivo disponible para descargar."
                );
            }

            if (!Directory.Exists(targetDirectory))
            {
                Directory.CreateDirectory(targetDirectory);
            }

            var extension = GetExtensionFromMime(version.FileMime);
            var fileName = $"{artifactName}_v{version.VersionNumber}{extension}";
            var filePath = Path.Combine(targetDirectory, fileName);

            // Handle file name conflicts
            int counter = 1;
            while (File.Exists(filePath))
            {
                fileName = $"{artifactName}_v{version.VersionNumber}_{counter}{extension}";
                filePath = Path.Combine(targetDirectory, fileName);
                counter++;
            }

            await File.WriteAllBytesAsync(filePath, version.FileBlob, ct);

            _logger.LogInformation("Artifact version {VersionId} downloaded to {FilePath}", version.Id, filePath);

            return new DownloadResult(
                Success: true,
                Message: "Archivo descargado exitosamente.",
                FilePath: filePath
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error downloading artifact version {VersionId}", version.Id);
            return new DownloadResult(
                Success: false,
                Message: $"Error al descargar: {ex.Message}"
            );
        }
    }

    public string GetTempDirectory() => _tempDirectory;

    public void CleanupTempFiles()
    {
        try
        {
            if (Directory.Exists(_tempDirectory))
            {
                var files = Directory.GetFiles(_tempDirectory);
                foreach (var file in files)
                {
                    try
                    {
                        // Delete files older than 1 hour
                        var fileInfo = new FileInfo(file);
                        if (DateTime.Now - fileInfo.CreationTime > TimeSpan.FromHours(1))
                        {
                            File.Delete(file);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Failed to delete temp file {FilePath}", file);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error cleaning up temp files");
        }
    }

    private string GetExtensionFromMime(string? mimeType)
    {
        if (string.IsNullOrWhiteSpace(mimeType))
            return ".bin";

        return mimeType.ToLower() switch
        {
            // Documents
            "application/pdf" => ".pdf",
            "application/msword" => ".doc",
            "application/vnd.openxmlformats-officedocument.wordprocessingml.document" => ".docx",
            "application/vnd.ms-excel" => ".xls",
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" => ".xlsx",
            "application/vnd.ms-powerpoint" => ".ppt",
            "application/vnd.openxmlformats-officedocument.presentationml.presentation" => ".pptx",
            
            // Images
            "image/jpeg" => ".jpg",
            "image/png" => ".png",
            "image/gif" => ".gif",
            "image/bmp" => ".bmp",
            "image/svg+xml" => ".svg",
            "image/webp" => ".webp",
            
            // Text
            "text/plain" => ".txt",
            "text/html" => ".html",
            "text/css" => ".css",
            "text/javascript" => ".js",
            "application/json" => ".json",
            "application/xml" or "text/xml" => ".xml",
            
            // Archives
            "application/zip" => ".zip",
            "application/x-rar-compressed" => ".rar",
            "application/x-7z-compressed" => ".7z",
            "application/x-tar" => ".tar",
            "application/gzip" => ".gz",
            
            // Video
            "video/mp4" => ".mp4",
            "video/x-msvideo" => ".avi",
            "video/mpeg" => ".mpeg",
            "video/quicktime" => ".mov",
            
            // Audio
            "audio/mpeg" => ".mp3",
            "audio/wav" => ".wav",
            "audio/ogg" => ".ogg",
            
            _ => ".bin"
        };
    }

    private bool CanOpenDirectly(string? mimeType)
    {
        if (string.IsNullOrWhiteSpace(mimeType))
            return false;

        var openableMimes = new[]
        {
            "application/pdf",
            "image/jpeg", "image/png", "image/gif", "image/bmp", "image/webp",
            "text/plain", "text/html", "text/css", "text/javascript",
            "application/json", "application/xml", "text/xml",
            "video/mp4", "video/x-msvideo",
            "audio/mpeg", "audio/wav",
            "application/msword", "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
            "application/vnd.ms-excel", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            "application/vnd.ms-powerpoint", "application/vnd.openxmlformats-officedocument.presentationml.presentation"
        };

        return openableMimes.Contains(mimeType.ToLower());
    }

    private Task<PreviewResult> OpenFileAsync(string filePath)
    {
        try
        {
            var psi = new ProcessStartInfo
            {
                FileName = filePath,
                UseShellExecute = true
            };
            Process.Start(psi);

            return Task.FromResult(new PreviewResult(
                Success: true,
                Message: "Archivo abierto.",
                FilePath: filePath
            ));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error opening file {FilePath}", filePath);
            return Task.FromResult(new PreviewResult(
                Success: false,
                Message: $"Error al abrir archivo: {ex.Message}"
            ));
        }
    }

    private Task<PreviewResult> OpenInExplorerAsync(string filePath)
    {
        try
        {
            var argument = $"/select, \"{filePath}\"";
            Process.Start("explorer.exe", argument);

            return Task.FromResult(new PreviewResult(
                Success: true,
                Message: "Carpeta abierta en explorador.",
                FilePath: filePath
            ));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error opening explorer for {FilePath}", filePath);
            return Task.FromResult(new PreviewResult(
                Success: false,
                Message: $"Error al abrir carpeta: {ex.Message}"
            ));
        }
    }

    private Task<PreviewResult> OpenUrlAsync(string url)
    {
        try
        {
            var psi = new ProcessStartInfo
            {
                FileName = url,
                UseShellExecute = true
            };
            Process.Start(psi);

            return Task.FromResult(new PreviewResult(
                Success: true,
                Message: "URL abierta en navegador."
            ));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error opening URL {Url}", url);
            return Task.FromResult(new PreviewResult(
                Success: false,
                Message: $"Error al abrir URL: {ex.Message}"
            ));
        }
    }
}

