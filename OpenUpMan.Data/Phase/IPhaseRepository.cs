using OpenUpMan.Domain;

namespace OpenUpMan.Data
{
    public interface IPhaseRepository
    {
        Task<Phase?> GetByIdAsync(int id, CancellationToken ct = default);
        Task<IEnumerable<Phase>> GetAllAsync(CancellationToken ct = default);
        Task<IEnumerable<Phase>> GetByProjectIdAsync(int projectId, CancellationToken ct = default);
        Task AddAsync(Phase phase, CancellationToken ct = default);
        Task UpdateAsync(Phase phase, CancellationToken ct = default);
        Task DeleteAsync(int id, CancellationToken ct = default);
        Task<bool> ExistsAsync(int id, CancellationToken ct = default);
    }
}

