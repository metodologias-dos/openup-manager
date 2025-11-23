using OpenUpMan.Domain;

namespace OpenUpMan.Data
{
    public interface IProjectPhaseRepository
    {
        Task<ProjectPhase?> GetByIdAsync(Guid id, CancellationToken ct = default);
        Task<IEnumerable<ProjectPhase>> GetByProjectIdAsync(Guid projectId, CancellationToken ct = default);
        Task<ProjectPhase?> GetByProjectIdAndCodeAsync(Guid projectId, PhaseCode code, CancellationToken ct = default);
        Task AddAsync(ProjectPhase projectPhase, CancellationToken ct = default);
        Task UpdateAsync(ProjectPhase projectPhase, CancellationToken ct = default);
        Task DeleteAsync(ProjectPhase projectPhase, CancellationToken ct = default);
        Task SaveChangesAsync(CancellationToken ct = default);
    }
}

