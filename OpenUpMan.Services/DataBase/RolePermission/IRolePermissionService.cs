using OpenUpMan.Domain;

namespace OpenUpMan.Services
{
    public interface IRolePermissionService
    {
        Task<ServiceResult<RolePermission>> AssignPermissionToRoleAsync(Guid roleId, Guid permissionId, CancellationToken ct = default);
        Task<ServiceResult<IEnumerable<Permission>>> GetPermissionsByRoleIdAsync(Guid roleId, CancellationToken ct = default);
        Task<ServiceResult<IEnumerable<Role>>> GetRolesByPermissionIdAsync(Guid permissionId, CancellationToken ct = default);
        Task<ServiceResult<bool>> RemovePermissionFromRoleAsync(Guid roleId, Guid permissionId, CancellationToken ct = default);
        Task<ServiceResult<bool>> RoleHasPermissionAsync(Guid roleId, Guid permissionId, CancellationToken ct = default);
    }
}

