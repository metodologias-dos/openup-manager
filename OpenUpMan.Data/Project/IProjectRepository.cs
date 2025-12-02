using OpenUpMan.Domain;

namespace OpenUpMan.Data
{
    public interface IProjectRepository
    {
        Task<Project?> GetByIdAsync(int id, CancellationToken ct = default);
        Task<Project?> GetByCodeAsync(string code, CancellationToken ct = default);
        Task<IEnumerable<Project>> GetAllAsync(CancellationToken ct = default);
        Task<IEnumerable<Project>> GetAllIncludingDeletedAsync(CancellationToken ct = default);
        Task<IEnumerable<Project>> GetByCreatorAsync(int createdBy, CancellationToken ct = default);
        Task<IEnumerable<Project>> GetByStatusAsync(string status, CancellationToken ct = default);
        Task AddAsync(Project project, CancellationToken ct = default);
        Task UpdateAsync(Project project, CancellationToken ct = default);
        Task DeleteAsync(int id, CancellationToken ct = default);
        Task<int> SaveChangesAsync(CancellationToken ct = default);
        Task<string?> GetLastProjectCodeAsync(CancellationToken ct = default);
    }
}
