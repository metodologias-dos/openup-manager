using OpenUpMan.Domain;

namespace OpenUpMan.Services
{
    public interface IPhaseService
    {
        Task<PhaseServiceResult> CreatePhaseAsync(int projectId, string name, int orderIndex, string status = "PENDING", CancellationToken ct = default);
        Task<PhaseServiceResult> GetPhaseByIdAsync(int id, CancellationToken ct = default);
        Task<IEnumerable<Phase>> GetPhasesByProjectIdAsync(int projectId, CancellationToken ct = default);
        Task<PhaseServiceResult> UpdatePhaseAsync(int id, string name, DateTime? startDate, DateTime? endDate, string status, CancellationToken ct = default);
        Task<PhaseServiceResult> SetPhaseStatusAsync(int id, string status, CancellationToken ct = default);
        Task<PhaseServiceResult> DeletePhaseAsync(int id, CancellationToken ct = default);
    }

    public record PhaseServiceResult(
        bool Success,
        ServiceResultType ResultType,
        string Message,
        Phase? Phase = null
    );
}

