using OpenUpMan.Domain;

namespace OpenUpMan.Data
{
    public interface IRolePermissionRepository
    {
        Task<IEnumerable<Permission>> GetPermissionsByRoleIdAsync(int roleId, CancellationToken ct = default);
        Task<IEnumerable<Permission>> GetPermissionsByRoleAsync(int roleId, CancellationToken ct = default);
        Task<IEnumerable<Permission>> GetPermissionsByRolesAsync(IEnumerable<int> roleIds, CancellationToken ct = default);
        Task<bool> RoleHasPermissionAsync(IEnumerable<int> roleIds, int permissionId, CancellationToken ct = default);
        Task<IEnumerable<Role>> GetRolesByPermissionIdAsync(int permissionId, CancellationToken ct = default);
        Task<IEnumerable<RolePermission>> GetAllAsync(CancellationToken ct = default);
        Task AddAsync(RolePermission rolePermission, CancellationToken ct = default);
        Task DeleteAsync(int roleId, int permissionId, CancellationToken ct = default);
        Task<bool> ExistsAsync(int roleId, int permissionId, CancellationToken ct = default);
        Task SaveChangesAsync(CancellationToken ct = default);
    }
}

