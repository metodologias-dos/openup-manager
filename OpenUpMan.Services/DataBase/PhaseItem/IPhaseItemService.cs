using OpenUpMan.Domain;

namespace OpenUpMan.Services
{
    public interface IPhaseItemService
    {
        Task<PhaseItemServiceResult> CreateIterationAsync(Guid projectPhaseId, int number, string name, Guid createdBy, string? description = null, DateTime? startDate = null, DateTime? endDate = null, CancellationToken ct = default);
        Task<PhaseItemServiceResult> CreateMicroincrementAsync(Guid projectPhaseId, Guid parentIterationId, int number, string name, Guid createdBy, string? description = null, DateTime? startDate = null, CancellationToken ct = default);
        Task<PhaseItemServiceResult> GetPhaseItemByIdAsync(Guid id, CancellationToken ct = default);
        Task<IEnumerable<PhaseItem>> GetPhaseItemsByPhaseAsync(Guid projectPhaseId, CancellationToken ct = default);
        Task<IEnumerable<PhaseItem>> GetIterationsByPhaseAsync(Guid projectPhaseId, CancellationToken ct = default);
        Task<IEnumerable<PhaseItem>> GetMicroincrementsByIterationAsync(Guid iterationId, CancellationToken ct = default);
        Task<PhaseItemServiceResult> UpdatePhaseItemAsync(Guid id, string name, string? description, DateTime? startDate, DateTime? endDate, CancellationToken ct = default);
        Task<PhaseItemServiceResult> ChangePhaseItemStateAsync(Guid id, PhaseItemState newState, CancellationToken ct = default);
    }
}

