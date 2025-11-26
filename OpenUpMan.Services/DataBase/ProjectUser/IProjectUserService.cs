using OpenUpMan.Domain;

namespace OpenUpMan.Services
{
    public interface IProjectUserService
    {
        Task<ProjectUserServiceResult> AddUserToProjectAsync(Guid projectId, Guid userId, ProjectUserPermission permissions = ProjectUserPermission.VIEWER, ProjectUserRole role = ProjectUserRole.AUTOR, CancellationToken ct = default);
        Task<ProjectUserServiceResult> RemoveUserFromProjectAsync(Guid projectId, Guid userId, CancellationToken ct = default);
        Task<ProjectUserServiceResult> ChangeUserRoleAsync(Guid projectId, Guid userId, ProjectUserRole newRole, CancellationToken ct = default);
        Task<ProjectUserServiceResult> ChangeUserPermissionsAsync(Guid projectId, Guid userId, ProjectUserPermission newPermissions, CancellationToken ct = default);
        Task<IEnumerable<ProjectUser>> GetProjectUsersAsync(Guid projectId, CancellationToken ct = default);
        Task<IEnumerable<ProjectUser>> GetUserProjectsAsync(Guid userId, CancellationToken ct = default);
    }
}

