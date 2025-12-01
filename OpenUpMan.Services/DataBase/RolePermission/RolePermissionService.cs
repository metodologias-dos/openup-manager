using Microsoft.Extensions.Logging;
using OpenUpMan.Data.Repositories;
using OpenUpMan.Domain;

namespace OpenUpMan.Services
{
    public class RolePermissionService : IRolePermissionService
    {
        private readonly IRolePermissionRepository _repo;
        private readonly ILogger<RolePermissionService> _logger;

        public RolePermissionService(IRolePermissionRepository repo, ILogger<RolePermissionService> logger)
        {
            _repo = repo ?? throw new ArgumentNullException(nameof(repo));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<ServiceResult<RolePermission>> AssignPermissionToRoleAsync(Guid roleId, Guid permissionId, CancellationToken ct = default)
        {
            try
            {
                if (roleId == Guid.Empty)
                {
                    return new ServiceResult<RolePermission>(
                        Success: false,
                        ResultType: ServiceResultType.Error,
                        Message: "ID de rol inválido.",
                        Data: null
                    );
                }

                if (permissionId == Guid.Empty)
                {
                    return new ServiceResult<RolePermission>(
                        Success: false,
                        ResultType: ServiceResultType.Error,
                        Message: "ID de permiso inválido.",
                        Data: null
                    );
                }

                var exists = await _repo.ExistsAsync(roleId, permissionId);
                if (exists)
                {
                    return new ServiceResult<RolePermission>(
                        Success: false,
                        ResultType: ServiceResultType.Error,
                        Message: "El permiso ya está asignado a este rol.",
                        Data: null
                    );
                }

                var rolePermission = new RolePermission(roleId, permissionId);
                await _repo.AddAsync(rolePermission);

                _logger.LogInformation("Permiso {PermissionId} asignado al rol {RoleId}", permissionId, roleId);

                return new ServiceResult<RolePermission>(
                    Success: true,
                    ResultType: ServiceResultType.Success,
                    Message: "Permiso asignado al rol exitosamente.",
                    Data: rolePermission
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al asignar permiso {PermissionId} al rol {RoleId}", permissionId, roleId);
                return new ServiceResult<RolePermission>(
                    Success: false,
                    ResultType: ServiceResultType.Error,
                    Message: "Error al asignar el permiso al rol.",
                    Data: null
                );
            }
        }

        public async Task<ServiceResult<IEnumerable<Permission>>> GetPermissionsByRoleIdAsync(Guid roleId, CancellationToken ct = default)
        {
            try
            {
                if (roleId == Guid.Empty)
                {
                    return new ServiceResult<IEnumerable<Permission>>(
                        Success: false,
                        ResultType: ServiceResultType.Error,
                        Message: "ID de rol inválido.",
                        Data: null
                    );
                }

                var permissions = await _repo.GetPermissionsByRoleIdAsync(roleId);
                return new ServiceResult<IEnumerable<Permission>>(
                    Success: true,
                    ResultType: ServiceResultType.Success,
                    Message: $"Se encontraron {permissions.Count()} permisos para el rol.",
                    Data: permissions
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener permisos del rol {RoleId}", roleId);
                return new ServiceResult<IEnumerable<Permission>>(
                    Success: false,
                    ResultType: ServiceResultType.Error,
                    Message: "Error al obtener los permisos del rol.",
                    Data: null
                );
            }
        }

        public async Task<ServiceResult<IEnumerable<Role>>> GetRolesByPermissionIdAsync(Guid permissionId, CancellationToken ct = default)
        {
            try
            {
                if (permissionId == Guid.Empty)
                {
                    return new ServiceResult<IEnumerable<Role>>(
                        Success: false,
                        ResultType: ServiceResultType.Error,
                        Message: "ID de permiso inválido.",
                        Data: null
                    );
                }

                var roles = await _repo.GetRolesByPermissionIdAsync(permissionId);
                return new ServiceResult<IEnumerable<Role>>(
                    Success: true,
                    ResultType: ServiceResultType.Success,
                    Message: $"Se encontraron {roles.Count()} roles con el permiso.",
                    Data: roles
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener roles con permiso {PermissionId}", permissionId);
                return new ServiceResult<IEnumerable<Role>>(
                    Success: false,
                    ResultType: ServiceResultType.Error,
                    Message: "Error al obtener los roles.",
                    Data: null
                );
            }
        }

        public async Task<ServiceResult<bool>> RemovePermissionFromRoleAsync(Guid roleId, Guid permissionId, CancellationToken ct = default)
        {
            try
            {
                if (roleId == Guid.Empty)
                {
                    return new ServiceResult<bool>(
                        Success: false,
                        ResultType: ServiceResultType.Error,
                        Message: "ID de rol inválido.",
                        Data: false
                    );
                }

                if (permissionId == Guid.Empty)
                {
                    return new ServiceResult<bool>(
                        Success: false,
                        ResultType: ServiceResultType.Error,
                        Message: "ID de permiso inválido.",
                        Data: false
                    );
                }

                var exists = await _repo.ExistsAsync(roleId, permissionId);
                if (!exists)
                {
                    return new ServiceResult<bool>(
                        Success: false,
                        ResultType: ServiceResultType.NotFound,
                        Message: "El permiso no está asignado a este rol.",
                        Data: false
                    );
                }

                await _repo.DeleteAsync(roleId, permissionId);

                _logger.LogInformation("Permiso {PermissionId} removido del rol {RoleId}", permissionId, roleId);

                return new ServiceResult<bool>(
                    Success: true,
                    ResultType: ServiceResultType.Success,
                    Message: "Permiso removido del rol exitosamente.",
                    Data: true
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al remover permiso {PermissionId} del rol {RoleId}", permissionId, roleId);
                return new ServiceResult<bool>(
                    Success: false,
                    ResultType: ServiceResultType.Error,
                    Message: "Error al remover el permiso del rol.",
                    Data: false
                );
            }
        }

        public async Task<ServiceResult<bool>> RoleHasPermissionAsync(Guid roleId, Guid permissionId, CancellationToken ct = default)
        {
            try
            {
                if (roleId == Guid.Empty || permissionId == Guid.Empty)
                {
                    return new ServiceResult<bool>(
                        Success: false,
                        ResultType: ServiceResultType.Error,
                        Message: "IDs inválidos.",
                        Data: false
                    );
                }

                var exists = await _repo.ExistsAsync(roleId, permissionId);
                return new ServiceResult<bool>(
                    Success: true,
                    ResultType: ServiceResultType.Success,
                    Message: exists ? "El rol tiene el permiso." : "El rol no tiene el permiso.",
                    Data: exists
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al verificar permiso {PermissionId} en rol {RoleId}", permissionId, roleId);
                return new ServiceResult<bool>(
                    Success: false,
                    ResultType: ServiceResultType.Error,
                    Message: "Error al verificar el permiso.",
                    Data: false
                );
            }
        }
    }
}

