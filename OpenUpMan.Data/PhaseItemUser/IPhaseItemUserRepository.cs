using OpenUpMan.Domain;

namespace OpenUpMan.Data
{
    public interface IPhaseItemUserRepository
    {
        Task<PhaseItemUser?> GetByIdAsync(Guid phaseItemId, Guid userId, CancellationToken ct = default);
        Task<IEnumerable<PhaseItemUser>> GetByPhaseItemIdAsync(Guid phaseItemId, CancellationToken ct = default);
        Task<IEnumerable<PhaseItemUser>> GetByUserIdAsync(Guid userId, CancellationToken ct = default);
        Task AddAsync(PhaseItemUser phaseItemUser, CancellationToken ct = default);
        Task RemoveAsync(PhaseItemUser phaseItemUser, CancellationToken ct = default);
        Task SaveChangesAsync(CancellationToken ct = default);
    }
}

