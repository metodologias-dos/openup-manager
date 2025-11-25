using OpenUpMan.Domain;

namespace OpenUpMan.Data
{
    public interface IProjectUserRepository
    {
        Task<ProjectUser?> GetByIdAsync(Guid projectId, Guid userId, CancellationToken ct = default);
        Task<IEnumerable<ProjectUser>> GetByProjectIdAsync(Guid projectId, CancellationToken ct = default);
        Task<IEnumerable<ProjectUser>> GetByUserIdAsync(Guid userId, CancellationToken ct = default);
        Task AddAsync(ProjectUser projectUser, CancellationToken ct = default);
        Task RemoveAsync(ProjectUser projectUser, CancellationToken ct = default);
        Task SaveChangesAsync(CancellationToken ct = default);
    }
}

