using OpenUpMan.Domain;

namespace OpenUpMan.Data
{
    public interface IArtifactRepository
    {
        Task<Artifact?> GetByIdAsync(int id, CancellationToken ct = default);
        Task<IEnumerable<Artifact>> GetAllAsync(CancellationToken ct = default);
        Task<IEnumerable<Artifact>> GetByProjectIdAsync(int projectId, CancellationToken ct = default);
        Task<IEnumerable<Artifact>> GetByPhaseIdAsync(int phaseId, CancellationToken ct = default);
        Task<IEnumerable<Artifact>> GetMandatoryByProjectIdAsync(int projectId, CancellationToken ct = default);
        Task AddAsync(Artifact artifact, CancellationToken ct = default);
        Task UpdateAsync(Artifact artifact, CancellationToken ct = default);
        Task DeleteAsync(int id, CancellationToken ct = default);
        Task<bool> ExistsAsync(int id, CancellationToken ct = default);
        
        // Artifact version operations
        Task<IEnumerable<ArtifactVersion>> GetVersionHistoryAsync(int artifactId, CancellationToken ct = default);
        Task<ArtifactVersion?> GetLatestVersionAsync(int artifactId, CancellationToken ct = default);
        Task<ArtifactVersion?> GetVersionAsync(int artifactId, int versionNumber, CancellationToken ct = default);
        Task<int> AddVersionAsync(ArtifactVersion version, CancellationToken ct = default);
    }
}

