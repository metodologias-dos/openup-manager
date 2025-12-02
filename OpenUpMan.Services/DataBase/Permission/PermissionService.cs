using Microsoft.Extensions.Logging;
using OpenUpMan.Data;
using OpenUpMan.Domain;

namespace OpenUpMan.Services
{
    /// <summary>
    /// Servicio para verificar y gestionar permisos de usuarios en proyectos
    /// </summary>
    public class PermissionService : IPermissionService
    {
        private readonly IPermissionRepository _permissionRepo;
        private readonly IRoleRepository _roleRepo;
        private readonly IRolePermissionRepository _rolePermissionRepo;
        private readonly IProjectUserRepository _projectUserRepo;
        private readonly ILogger<PermissionService> _logger;

        public PermissionService(
            IPermissionRepository permissionRepo,
            IRoleRepository roleRepo,
            IRolePermissionRepository rolePermissionRepo,
            IProjectUserRepository projectUserRepo,
            ILogger<PermissionService> logger)
        {
            _permissionRepo = permissionRepo;
            _roleRepo = roleRepo;
            _rolePermissionRepo = rolePermissionRepo;
            _projectUserRepo = projectUserRepo;
            _logger = logger;
        }

        public async Task<PermissionServiceResult> CheckPermissionAsync(int userId, int projectId, int permissionId, CancellationToken ct = default)
        {
            try
            {
                var projectUsers = await _projectUserRepo.GetByUserAndProjectAsync(userId, projectId, ct);
                if (!projectUsers.Any())
                {
                    return new PermissionServiceResult(
                        Success: true,
                        ResultType: PermissionServiceResultType.Success,
                        Message: "Usuario no pertenece al proyecto.",
                        HasPermission: false
                    );
                }

                var roleIds = projectUsers.Select(pu => pu.RoleId).ToList();
                var hasPermission = await _rolePermissionRepo.RoleHasPermissionAsync(roleIds, permissionId, ct);

                return new PermissionServiceResult(
                    Success: true,
                    ResultType: PermissionServiceResultType.Success,
                    Message: hasPermission ? "Permiso otorgado." : "Permiso denegado.",
                    HasPermission: hasPermission
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error verificando permiso {PermissionId} para usuario {UserId} en proyecto {ProjectId}", permissionId, userId, projectId);
                return new PermissionServiceResult(
                    Success: false,
                    ResultType: PermissionServiceResultType.Error,
                    Message: "Error al verificar el permiso."
                );
            }
        }

        public async Task<PermissionServiceResult> CheckPermissionAsync(int userId, int projectId, string permissionName, CancellationToken ct = default)
        {
            try
            {
                var permission = await _permissionRepo.GetByNameAsync(permissionName, ct);
                if (permission == null)
                {
                    return new PermissionServiceResult(
                        Success: false,
                        ResultType: PermissionServiceResultType.NotFound,
                        Message: $"Permiso '{permissionName}' no encontrado."
                    );
                }

                return await CheckPermissionAsync(userId, projectId, permission.Id, ct);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error verificando permiso '{PermissionName}' para usuario {UserId} en proyecto {ProjectId}", permissionName, userId, projectId);
                return new PermissionServiceResult(
                    Success: false,
                    ResultType: PermissionServiceResultType.Error,
                    Message: "Error al verificar el permiso."
                );
            }
        }

        public async Task<PermissionListResult> GetUserPermissionsAsync(int userId, int projectId, CancellationToken ct = default)
        {
            try
            {
                var projectUsers = await _projectUserRepo.GetByUserAndProjectAsync(userId, projectId, ct);
                if (!projectUsers.Any())
                {
                    return new PermissionListResult(
                        Success: true,
                        ResultType: PermissionServiceResultType.Success,
                        Message: "Usuario no pertenece al proyecto.",
                        Permissions: Enumerable.Empty<Permission>()
                    );
                }

                var roleIds = projectUsers.Select(pu => pu.RoleId).ToList();
                var permissions = await _rolePermissionRepo.GetPermissionsByRolesAsync(roleIds, ct);

                return new PermissionListResult(
                    Success: true,
                    ResultType: PermissionServiceResultType.Success,
                    Message: $"Se encontraron {permissions.Count()} permisos.",
                    Permissions: permissions
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error obteniendo permisos para usuario {UserId} en proyecto {ProjectId}", userId, projectId);
                return new PermissionListResult(
                    Success: false,
                    ResultType: PermissionServiceResultType.Error,
                    Message: "Error al obtener los permisos."
                );
            }
        }

        public async Task<RoleListResult> GetUserRolesAsync(int userId, int projectId, CancellationToken ct = default)
        {
            try
            {
                var projectUsers = await _projectUserRepo.GetByUserAndProjectAsync(userId, projectId, ct);
                if (!projectUsers.Any())
                {
                    return new RoleListResult(
                        Success: true,
                        ResultType: PermissionServiceResultType.Success,
                        Message: "Usuario no pertenece al proyecto.",
                        Roles: Enumerable.Empty<Role>()
                    );
                }

                var roleIds = projectUsers.Select(pu => pu.RoleId).Distinct().ToList();
                var roles = await _roleRepo.GetByIdsAsync(roleIds, ct);

                return new RoleListResult(
                    Success: true,
                    ResultType: PermissionServiceResultType.Success,
                    Message: $"Se encontraron {roles.Count()} roles.",
                    Roles: roles
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error obteniendo roles para usuario {UserId} en proyecto {ProjectId}", userId, projectId);
                return new RoleListResult(
                    Success: false,
                    ResultType: PermissionServiceResultType.Error,
                    Message: "Error al obtener los roles."
                );
            }
        }

        public async Task<PermissionListResult> GetAllPermissionsAsync(CancellationToken ct = default)
        {
            try
            {
                var permissions = await _permissionRepo.GetAllAsync(ct);
                return new PermissionListResult(
                    Success: true,
                    ResultType: PermissionServiceResultType.Success,
                    Message: $"Se encontraron {permissions.Count()} permisos.",
                    Permissions: permissions
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error obteniendo todos los permisos");
                return new PermissionListResult(
                    Success: false,
                    ResultType: PermissionServiceResultType.Error,
                    Message: "Error al obtener los permisos."
                );
            }
        }

        public async Task<RoleListResult> GetAllRolesAsync(CancellationToken ct = default)
        {
            try
            {
                var roles = await _roleRepo.GetAllAsync(ct);
                return new RoleListResult(
                    Success: true,
                    ResultType: PermissionServiceResultType.Success,
                    Message: $"Se encontraron {roles.Count()} roles.",
                    Roles: roles
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error obteniendo todos los roles");
                return new RoleListResult(
                    Success: false,
                    ResultType: PermissionServiceResultType.Error,
                    Message: "Error al obtener los roles."
                );
            }
        }

        public async Task<PermissionListResult> GetRolePermissionsAsync(int roleId, CancellationToken ct = default)
        {
            try
            {
                var permissions = await _rolePermissionRepo.GetPermissionsByRoleAsync(roleId, ct);
                return new PermissionListResult(
                    Success: true,
                    ResultType: PermissionServiceResultType.Success,
                    Message: $"Se encontraron {permissions.Count()} permisos para el rol.",
                    Permissions: permissions
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error obteniendo permisos para rol {RoleId}", roleId);
                return new PermissionListResult(
                    Success: false,
                    ResultType: PermissionServiceResultType.Error,
                    Message: "Error al obtener los permisos del rol."
                );
            }
        }

        public async Task<PermissionServiceResult> CheckReadOnlyAccessAsync(int userId, int projectId, CancellationToken ct = default)
        {
            return await CheckPermissionAsync(userId, projectId, PermissionIds.SoloLectura, ct);
        }

        public async Task<PermissionServiceResult> CheckIsAdminOrAuthorAsync(int userId, int projectId, CancellationToken ct = default)
        {
            try
            {
                var projectUsers = await _projectUserRepo.GetByUserAndProjectAsync(userId, projectId, ct);
                var roleIds = projectUsers.Select(pu => pu.RoleId).ToList();
                
                var isAdminOrAuthor = roleIds.Contains(RoleIds.Admin) || roleIds.Contains(RoleIds.Autor);

                return new PermissionServiceResult(
                    Success: true,
                    ResultType: PermissionServiceResultType.Success,
                    Message: isAdminOrAuthor ? "Usuario es Admin/Autor." : "Usuario no es Admin/Autor.",
                    HasPermission: isAdminOrAuthor
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error verificando si usuario {UserId} es Admin/Autor en proyecto {ProjectId}", userId, projectId);
                return new PermissionServiceResult(
                    Success: false,
                    ResultType: PermissionServiceResultType.Error,
                    Message: "Error al verificar el rol."
                );
            }
        }

        public async Task<PermissionCheckResult> CheckMultiplePermissionsAsync(int userId, int projectId, CancellationToken ct = default, params int[] permissionIds)
        {
            try
            {
                var userPermissionsResult = await GetUserPermissionsAsync(userId, projectId, ct);
                if (!userPermissionsResult.Success || userPermissionsResult.Permissions == null)
                {
                    return new PermissionCheckResult(
                        Success: false,
                        ResultType: PermissionServiceResultType.Error,
                        Message: "Error al obtener permisos del usuario."
                    );
                }

                var userPermissionIds = userPermissionsResult.Permissions.Select(p => p.Id).ToHashSet();
                var checks = permissionIds.ToDictionary(
                    permId => permId,
                    permId => userPermissionIds.Contains(permId)
                );

                return new PermissionCheckResult(
                    Success: true,
                    ResultType: PermissionServiceResultType.Success,
                    Message: $"Se verificaron {permissionIds.Length} permisos.",
                    PermissionChecks: checks
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error verificando múltiples permisos para usuario {UserId} en proyecto {ProjectId}", userId, projectId);
                return new PermissionCheckResult(
                    Success: false,
                    ResultType: PermissionServiceResultType.Error,
                    Message: "Error al verificar los permisos."
                );
            }
        }

        public async Task<PermissionCountResult> CountUserPermissionsAsync(int userId, int projectId, CancellationToken ct = default)
        {
            try
            {
                var permissionsResult = await GetUserPermissionsAsync(userId, projectId, ct);
                if (!permissionsResult.Success)
                {
                    return new PermissionCountResult(
                        Success: false,
                        ResultType: PermissionServiceResultType.Error,
                        Message: "Error al contar permisos."
                    );
                }

                var count = permissionsResult.Permissions?.Count() ?? 0;
                return new PermissionCountResult(
                    Success: true,
                    ResultType: PermissionServiceResultType.Success,
                    Message: $"Usuario tiene {count} permisos.",
                    Count: count
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error contando permisos para usuario {UserId} en proyecto {ProjectId}", userId, projectId);
                return new PermissionCountResult(
                    Success: false,
                    ResultType: PermissionServiceResultType.Error,
                    Message: "Error al contar los permisos."
                );
            }
        }
    }
}

