using OpenUpMan.Domain;

namespace OpenUpMan.Services
{
    public interface IProjectService
    {
        Task<ProjectServiceResult> CreateProjectAsync(string identifier, string name, DateTime startDate, Guid ownerId, string? description = null, CancellationToken ct = default);
        Task<ProjectServiceResult> GetProjectByIdentifierAsync(string identifier, CancellationToken ct = default);
        Task<ProjectServiceResult> GetProjectsByOwnerAsync(Guid ownerId, CancellationToken ct = default);
        Task<ProjectServiceResult> UpdateProjectAsync(string identifier, string name, string? description, DateTime startDate, CancellationToken ct = default);
        Task<ProjectServiceResult> ChangeProjectStateAsync(string identifier, ProjectState newState, CancellationToken ct = default);
    }
}

