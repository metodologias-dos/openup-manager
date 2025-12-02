using OpenUpMan.Domain;

namespace OpenUpMan.Services
{
    public interface IMicroincrementService
    {
        Task<MicroincrementServiceResult> CreateMicroincrementAsync(int iterationId, string title, string? description, int? authorId, string type, int? artifactId = null, string? evidenceUrl = null, CancellationToken ct = default);
        Task<MicroincrementServiceResult> GetMicroincrementByIdAsync(int id, CancellationToken ct = default);
        Task<IEnumerable<Microincrement>> GetMicroincrementsByIterationIdAsync(int iterationId, CancellationToken ct = default);
        Task<IEnumerable<Microincrement>> GetMicroincrementsByAuthorAsync(int authorId, CancellationToken ct = default);
        Task<IEnumerable<Microincrement>> GetMicroincrementsByArtifactAsync(int artifactId, CancellationToken ct = default);
        Task<MicroincrementServiceResult> UpdateMicroincrementAsync(int id, string title, string? description, string? evidenceUrl, CancellationToken ct = default);
        Task<MicroincrementServiceResult> DeleteMicroincrementAsync(int id, CancellationToken ct = default);
    }

    public record MicroincrementServiceResult(
        bool Success,
        ServiceResultType ResultType,
        string Message,
        Microincrement? Microincrement = null
    );
}

