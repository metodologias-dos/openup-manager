using OpenUpMan.Domain;

namespace OpenUpMan.Services
{
    public interface IPermissionService
    {
        Task<ServiceResult<Permission>> CreatePermissionAsync(string name, string? description = null, CancellationToken ct = default);
        Task<ServiceResult<Permission>> GetPermissionByIdAsync(Guid id, CancellationToken ct = default);
        Task<ServiceResult<Permission>> GetPermissionByNameAsync(string name, CancellationToken ct = default);
        Task<ServiceResult<IEnumerable<Permission>>> GetAllPermissionsAsync(CancellationToken ct = default);
        Task<ServiceResult<Permission>> UpdatePermissionAsync(Guid id, string name, string? description = null, CancellationToken ct = default);
        Task<ServiceResult<bool>> DeletePermissionAsync(Guid id, CancellationToken ct = default);
    }
}

