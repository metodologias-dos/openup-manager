using OpenUpMan.Domain;

namespace OpenUpMan.Services;

public interface IArtifactPreviewService
{
    Task<PreviewResult> PreviewArtifactVersionAsync(ArtifactVersion version, string artifactName, CancellationToken ct = default);
    Task<DownloadResult> DownloadArtifactVersionAsync(ArtifactVersion version, string artifactName, string targetDirectory, CancellationToken ct = default);
    string GetTempDirectory();
    void CleanupTempFiles();
}

public record PreviewResult(
    bool Success,
    string Message,
    string? FilePath = null
);

public record DownloadResult(
    bool Success,
    string Message,
    string? FilePath = null
);

