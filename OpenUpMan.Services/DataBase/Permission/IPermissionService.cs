using OpenUpMan.Domain;

namespace OpenUpMan.Services
{
    public interface IPermissionService
    {
        Task<PermissionServiceResult> CheckPermissionAsync(int userId, int projectId, int permissionId, CancellationToken ct = default);
        Task<PermissionServiceResult> CheckPermissionAsync(int userId, int projectId, string permissionName, CancellationToken ct = default);
        Task<PermissionListResult> GetUserPermissionsAsync(int userId, int projectId, CancellationToken ct = default);
        Task<RoleListResult> GetUserRolesAsync(int userId, int projectId, CancellationToken ct = default);
        Task<PermissionListResult> GetAllPermissionsAsync(CancellationToken ct = default);
        Task<RoleListResult> GetAllRolesAsync(CancellationToken ct = default);
        Task<PermissionListResult> GetRolePermissionsAsync(int roleId, CancellationToken ct = default);
        Task<PermissionServiceResult> CheckReadOnlyAccessAsync(int userId, int projectId, CancellationToken ct = default);
        Task<PermissionServiceResult> CheckIsAdminOrAuthorAsync(int userId, int projectId, CancellationToken ct = default);
        Task<PermissionCheckResult> CheckMultiplePermissionsAsync(int userId, int projectId, CancellationToken ct = default, params int[] permissionIds);
        Task<PermissionCountResult> CountUserPermissionsAsync(int userId, int projectId, CancellationToken ct = default);
    }

    public record PermissionServiceResult(
        bool Success,
        PermissionServiceResultType ResultType,
        string Message,
        bool HasPermission = false
    );

    public record PermissionListResult(
        bool Success,
        PermissionServiceResultType ResultType,
        string Message,
        IEnumerable<Permission>? Permissions = null
    );

    public record RoleListResult(
        bool Success,
        PermissionServiceResultType ResultType,
        string Message,
        IEnumerable<Role>? Roles = null
    );

    public record PermissionCheckResult(
        bool Success,
        PermissionServiceResultType ResultType,
        string Message,
        Dictionary<int, bool>? PermissionChecks = null
    );

    public record PermissionCountResult(
        bool Success,
        PermissionServiceResultType ResultType,
        string Message,
        int Count = 0
    );
}

