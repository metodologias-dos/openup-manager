using OpenUpMan.Domain;

namespace OpenUpMan.Data
{
    public interface IPhaseItemRepository
    {
        Task<PhaseItem?> GetByIdAsync(Guid id, CancellationToken ct = default);
        Task<IEnumerable<PhaseItem>> GetByPhaseIdAsync(Guid projectPhaseId, CancellationToken ct = default);
        Task<IEnumerable<PhaseItem>> GetIterationsByPhaseIdAsync(Guid projectPhaseId, CancellationToken ct = default);
        Task<IEnumerable<PhaseItem>> GetMicroincrementsByIterationIdAsync(Guid iterationId, CancellationToken ct = default);
        Task<IEnumerable<PhaseItem>> GetByTypeAsync(Guid projectPhaseId, PhaseItemType type, CancellationToken ct = default);
        Task AddAsync(PhaseItem phaseItem, CancellationToken ct = default);
        Task UpdateAsync(PhaseItem phaseItem, CancellationToken ct = default);
        Task SaveChangesAsync(CancellationToken ct = default);
    }
}

