using OpenUpMan.Domain;

namespace OpenUpMan.Data
{
    public interface IMicroincrementRepository
    {
        Task<Microincrement?> GetByIdAsync(int id, CancellationToken ct = default);
        Task<IEnumerable<Microincrement>> GetAllAsync(CancellationToken ct = default);
        Task<IEnumerable<Microincrement>> GetByIterationIdAsync(int iterationId, CancellationToken ct = default);
        Task<IEnumerable<Microincrement>> GetByAuthorIdAsync(int authorId, CancellationToken ct = default);
        Task<IEnumerable<Microincrement>> GetByArtifactIdAsync(int artifactId, CancellationToken ct = default);
        Task<IEnumerable<Microincrement>> GetByProjectIdAsync(int projectId, CancellationToken ct = default);
        Task AddAsync(Microincrement microincrement, CancellationToken ct = default);
        Task UpdateAsync(Microincrement microincrement, CancellationToken ct = default);
        Task DeleteAsync(int id, CancellationToken ct = default);
        Task<bool> ExistsAsync(int id, CancellationToken ct = default);
    }
}

