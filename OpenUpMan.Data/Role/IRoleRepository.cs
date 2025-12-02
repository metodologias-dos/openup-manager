using OpenUpMan.Domain;

namespace OpenUpMan.Data
{
    public interface IRoleRepository
    {
        Task<Role?> GetByIdAsync(int id, CancellationToken ct = default);
        Task<Role?> GetByNameAsync(string name, CancellationToken ct = default);
        Task<IEnumerable<Role>> GetAllAsync(CancellationToken ct = default);
        Task AddAsync(Role role, CancellationToken ct = default);
        Task UpdateAsync(Role role, CancellationToken ct = default);
        Task DeleteAsync(int id, CancellationToken ct = default);
        Task<bool> ExistsAsync(int id, CancellationToken ct = default);
        Task SaveChangesAsync(CancellationToken ct = default);
    }
}

