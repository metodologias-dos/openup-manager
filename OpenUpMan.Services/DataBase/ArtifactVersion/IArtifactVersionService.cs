using OpenUpMan.Domain;

namespace OpenUpMan.Services
{
    public interface IArtifactVersionService
    {
        Task<ArtifactVersionServiceResult> CreateVersionAsync(int artifactId, int? createdBy, string? notes = null, byte[]? fileBlob = null, string? fileMime = null, string? buildInfo = null, CancellationToken ct = default);
        Task<ArtifactVersionServiceResult> GetLatestVersionAsync(int artifactId, CancellationToken ct = default);
        Task<ArtifactVersionServiceResult> GetVersionAsync(int artifactId, int versionNumber, CancellationToken ct = default);
        Task<IEnumerable<ArtifactVersion>> GetVersionHistoryAsync(int artifactId, CancellationToken ct = default);
    }

    public record ArtifactVersionServiceResult(
        bool Success,
        ServiceResultType ResultType,
        string Message,
        ArtifactVersion? ArtifactVersion = null,
        int? VersionNumber = null
    );
}

