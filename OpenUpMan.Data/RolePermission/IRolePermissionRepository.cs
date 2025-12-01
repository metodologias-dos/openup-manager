
using OpenUpMan.Domain;

namespace OpenUpMan.Data.Repositories
{
    public interface IRolePermissionRepository
    {
        Task<IEnumerable<Permission>> GetPermissionsByRoleIdAsync(Guid roleId);
        Task<IEnumerable<Role>> GetRolesByPermissionIdAsync(Guid permissionId);
        Task AddAsync(RolePermission rolePermission);
        Task DeleteAsync(Guid roleId, Guid permissionId);
        Task<bool> ExistsAsync(Guid roleId, Guid permissionId);
    }
}

