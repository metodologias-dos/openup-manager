using OpenUpMan.Domain;

namespace OpenUpMan.Services
{
    public interface IIterationService
    {
        Task<IterationServiceResult> CreateIterationAsync(int phaseId, string? name, string? goal, DateTime? startDate, DateTime? endDate, CancellationToken ct = default);
        Task<IterationServiceResult> GetIterationByIdAsync(int id, CancellationToken ct = default);
        Task<IEnumerable<Iteration>> GetIterationsByPhaseIdAsync(int phaseId, CancellationToken ct = default);
        Task<IterationServiceResult> UpdateIterationAsync(int id, string? name, string? goal, DateTime? startDate, DateTime? endDate, int completionPercentage, CancellationToken ct = default);
        Task<IterationServiceResult> SetCompletionAsync(int id, int percentage, CancellationToken ct = default);
        Task<IterationServiceResult> DeleteIterationAsync(int id, CancellationToken ct = default);
    }

    public record IterationServiceResult(
        bool Success,
        ServiceResultType ResultType,
        string Message,
        Iteration? Iteration = null
    );
}

