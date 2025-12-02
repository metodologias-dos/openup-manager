using OpenUpMan.Domain;

namespace OpenUpMan.Services
{
    public interface IRolePermissionService
    {
        Task<ServiceResult<RolePermission>> AssignPermissionToRoleAsync(int roleId, int permissionId, CancellationToken ct = default);
        Task<ServiceResult<IEnumerable<Permission>>> GetPermissionsByRoleIdAsync(int roleId, CancellationToken ct = default);
        Task<ServiceResult<IEnumerable<Role>>> GetRolesByPermissionIdAsync(int permissionId, CancellationToken ct = default);
        Task<ServiceResult<bool>> RemovePermissionFromRoleAsync(int roleId, int permissionId, CancellationToken ct = default);
        Task<ServiceResult<bool>> RoleHasPermissionAsync(int roleId, int permissionId, CancellationToken ct = default);
    }
}

