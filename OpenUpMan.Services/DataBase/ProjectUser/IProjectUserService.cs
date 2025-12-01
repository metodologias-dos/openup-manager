using OpenUpMan.Domain;

namespace OpenUpMan.Services
{
    public interface IProjectUserService
    {
        Task<ProjectUserServiceResult> AddUserToProjectAsync(Guid projectId, Guid userId, Guid roleId, CancellationToken ct = default);
        Task<ProjectUserServiceResult> RemoveUserFromProjectAsync(Guid projectId, Guid userId, CancellationToken ct = default);
        Task<ProjectUserServiceResult> ChangeUserRoleAsync(Guid projectId, Guid userId, Guid newRoleId, CancellationToken ct = default);
        Task<IEnumerable<ProjectUser>> GetProjectUsersAsync(Guid projectId, CancellationToken ct = default);
        Task<IEnumerable<ProjectUser>> GetUserProjectsAsync(Guid userId, CancellationToken ct = default);
    }
}

