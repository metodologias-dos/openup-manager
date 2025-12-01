using OpenUpMan.Domain;

namespace OpenUpMan.Services
{
    public interface IRoleService
    {
        Task<ServiceResult<Role>> CreateRoleAsync(string name, string? description = null, CancellationToken ct = default);
        Task<ServiceResult<Role>> GetRoleByIdAsync(Guid id, CancellationToken ct = default);
        Task<ServiceResult<Role>> GetRoleByNameAsync(string name, CancellationToken ct = default);
        Task<ServiceResult<IEnumerable<Role>>> GetAllRolesAsync(CancellationToken ct = default);
        Task<ServiceResult<Role>> UpdateRoleAsync(Guid id, string name, string? description = null, CancellationToken ct = default);
        Task<ServiceResult<bool>> DeleteRoleAsync(Guid id, CancellationToken ct = default);
    }
}

