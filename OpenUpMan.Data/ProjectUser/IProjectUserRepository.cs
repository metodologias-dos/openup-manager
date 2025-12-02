using OpenUpMan.Domain;

namespace OpenUpMan.Data
{
    public interface IProjectUserRepository
    {
        Task<ProjectUser?> GetByIdAsync(int id, CancellationToken ct = default);
        Task<ProjectUser?> GetByProjectAndUserAsync(int projectId, int userId, CancellationToken ct = default);
        Task<IEnumerable<ProjectUser>> GetByProjectIdAsync(int projectId, CancellationToken ct = default);
        Task<IEnumerable<ProjectUser>> GetByUserIdAsync(int userId, CancellationToken ct = default);
        Task<IEnumerable<ProjectUser>> GetByUserAndProjectAsync(int userId, int projectId, CancellationToken ct = default);
        Task AddAsync(ProjectUser projectUser, CancellationToken ct = default);
        Task UpdateAsync(ProjectUser projectUser, CancellationToken ct = default);
        Task RemoveAsync(int id, CancellationToken ct = default);
        Task SaveChangesAsync(CancellationToken ct = default);
    }
}

