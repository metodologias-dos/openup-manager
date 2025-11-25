using OpenUpMan.Domain;

namespace OpenUpMan.Services
{
    public interface IProjectPhaseService
    {
        Task<ProjectPhaseServiceResult> CreatePhaseAsync(Guid projectId, PhaseCode code, string name, int order, CancellationToken ct = default);
        Task<ProjectPhaseServiceResult> GetPhaseByIdAsync(Guid id, CancellationToken ct = default);
        Task<IEnumerable<ProjectPhase>> GetPhasesByProjectAsync(Guid projectId, CancellationToken ct = default);
        Task<ProjectPhaseServiceResult> UpdatePhaseAsync(Guid id, string name, int order, CancellationToken ct = default);
        Task<ProjectPhaseServiceResult> ChangePhaseStateAsync(Guid id, PhaseState newState, CancellationToken ct = default);
        Task<ProjectPhaseServiceResult> InitializeProjectPhasesAsync(Guid projectId, CancellationToken ct = default);
    }
}

