using OpenUpMan.Domain;

namespace OpenUpMan.Data
{
    public interface IPermissionRepository
    {
        Task<Permission?> GetByIdAsync(int id, CancellationToken ct = default);
        Task<Permission?> GetByNameAsync(string name, CancellationToken ct = default);
        Task<IEnumerable<Permission>> GetAllAsync(CancellationToken ct = default);
        Task AddAsync(Permission permission, CancellationToken ct = default);
        Task UpdateAsync(Permission permission, CancellationToken ct = default);
        Task DeleteAsync(int id, CancellationToken ct = default);
        Task<bool> ExistsAsync(int id, CancellationToken ct = default);
        Task SaveChangesAsync(CancellationToken ct = default);
    }
}

