using OpenUpMan.Domain;

namespace OpenUpMan.Services
{
    public interface IArtifactService
    {
        Task<ArtifactServiceResult> CreateArtifactAsync(int projectId, int phaseId, string name, string? artifactType = null, bool mandatory = false, string? description = null, CancellationToken ct = default);
        Task<ArtifactServiceResult> GetArtifactByIdAsync(int id, CancellationToken ct = default);
        Task<IEnumerable<Artifact>> GetArtifactsByProjectIdAsync(int projectId, CancellationToken ct = default);
        Task<IEnumerable<Artifact>> GetArtifactsByPhaseIdAsync(int phaseId, CancellationToken ct = default);
        Task<IEnumerable<Artifact>> GetMandatoryArtifactsByProjectIdAsync(int projectId, CancellationToken ct = default);
        Task<ArtifactServiceResult> UpdateArtifactAsync(int id, string name, string? artifactType, bool mandatory, string? description, CancellationToken ct = default);
        Task<ArtifactServiceResult> SetStateAsync(int id, string state, CancellationToken ct = default);
        Task<ArtifactServiceResult> DeleteArtifactAsync(int id, CancellationToken ct = default);
    }

    public record ArtifactServiceResult(
        bool Success,
        ServiceResultType ResultType,
        string Message,
        Artifact? Artifact = null
    );
}

