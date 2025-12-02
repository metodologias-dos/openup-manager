using OpenUpMan.Domain;

namespace OpenUpMan.Services
{
    public interface IProjectService
    {
        Task<ProjectServiceResult> CreateProjectAsync(string name, int? createdBy, string? code = null, string? description = null, DateTime? startDate = null, bool createPhases = true, CancellationToken ct = default);
        Task<ProjectServiceResult> GetProjectByIdAsync(int id, CancellationToken ct = default);
        Task<ProjectServiceResult> GetProjectByCodeAsync(string code, CancellationToken ct = default);
        Task<IEnumerable<Project>> GetAllProjectsAsync(CancellationToken ct = default);
        Task<ProjectServiceResult> UpdateProjectAsync(int id, string name, string? description, DateTime? startDate, string? code = null, CancellationToken ct = default);
        Task<ProjectServiceResult> SetStatusAsync(int id, string status, CancellationToken ct = default);
        Task<ProjectServiceResult> DeleteProjectAsync(int id, CancellationToken ct = default);
    }

    public record ProjectServiceResult(
        bool Success,
        ServiceResultType ResultType,
        string Message,
        Project? Project = null
    );
}

