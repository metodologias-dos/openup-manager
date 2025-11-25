using OpenUpMan.Domain;

namespace OpenUpMan.Data
{
    public interface IProjectRepository
    {
        Task<Project?> GetByIdAsync(Guid id, CancellationToken ct = default);
        Task<Project?> GetByIdentifierAsync(string identifier, CancellationToken ct = default);
        Task<IEnumerable<Project>> GetByOwnerAsync(Guid ownerId, CancellationToken ct = default);
        Task AddAsync(Project project, CancellationToken ct = default);
        Task UpdateAsync(Project project, CancellationToken ct = default);
        Task SaveChangesAsync(CancellationToken ct = default);
    }
}
