using OpenUpMan.Domain;

namespace OpenUpMan.Services
{
    public interface IProjectUserService
    {
        Task<ProjectUserServiceResult> AddUserToProjectAsync(int projectId, int userId, int roleId, CancellationToken ct = default);
        Task<ProjectUserServiceResult> RemoveUserFromProjectAsync(int projectId, int userId, CancellationToken ct = default);
        Task<ProjectUserServiceResult> ChangeUserRoleAsync(int projectId, int userId, int newRoleId, CancellationToken ct = default);
        Task<IEnumerable<ProjectUser>> GetProjectUsersAsync(int projectId, CancellationToken ct = default);
        Task<IEnumerable<ProjectUser>> GetUserProjectsAsync(int userId, CancellationToken ct = default);
    }
}

