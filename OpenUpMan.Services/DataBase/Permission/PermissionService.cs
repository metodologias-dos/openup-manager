using Microsoft.Extensions.Logging;
using OpenUpMan.Data;
using OpenUpMan.Data.Repositories;
using OpenUpMan.Domain;

namespace OpenUpMan.Services
{
    public class PermissionService : IPermissionService
    {
        private readonly IPermissionRepository _repo;
        private readonly ILogger<PermissionService> _logger;

        public PermissionService(IPermissionRepository repo, ILogger<PermissionService> logger)
        {
            _repo = repo ?? throw new ArgumentNullException(nameof(repo));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<ServiceResult<Permission>> CreatePermissionAsync(string name, string? description = null, CancellationToken ct = default)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(name))
                {
                    return new ServiceResult<Permission>(
                        Success: false,
                        ResultType: ServiceResultType.Error,
                        Message: "El nombre del permiso es requerido.",
                        Data: null
                    );
                }

                var existing = await _repo.GetByNameAsync(name);
                if (existing != null)
                {
                    return new ServiceResult<Permission>(
                        Success: false,
                        ResultType: ServiceResultType.Error,
                        Message: $"Ya existe un permiso con el nombre '{name}'.",
                        Data: null
                    );
                }

                var permission = new Domain.Permission(name, description);
                await _repo.AddAsync(permission);

                _logger.LogInformation("Permiso creado exitosamente: {PermissionName} con ID: {PermissionId}", name, permission.Id);

                return new ServiceResult<Permission>(
                    Success: true,
                    ResultType: ServiceResultType.Success,
                    Message: "Permiso creado exitosamente.",
                    Data: permission
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear el permiso: {PermissionName}", name);
                return new ServiceResult<Permission>(
                    Success: false,
                    ResultType: ServiceResultType.Error,
                    Message: "Error al crear el permiso. Por favor, inténtelo de nuevo.",
                    Data: null
                );
            }
        }

        public async Task<ServiceResult<Permission>> GetPermissionByIdAsync(Guid id, CancellationToken ct = default)
        {
            try
            {
                if (id == Guid.Empty)
                {
                    return new ServiceResult<Permission>(
                        Success: false,
                        ResultType: ServiceResultType.Error,
                        Message: "ID de permiso inválido.",
                        Data: null
                    );
                }

                var permission = await _repo.GetByIdAsync(id);
                if (permission == null)
                {
                    return new ServiceResult<Permission>(
                        Success: false,
                        ResultType: ServiceResultType.NotFound,
                        Message: "Permiso no encontrado.",
                        Data: null
                    );
                }

                return new ServiceResult<Permission>(
                    Success: true,
                    ResultType: ServiceResultType.Success,
                    Message: "Permiso encontrado.",
                    Data: permission
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el permiso con ID: {PermissionId}", id);
                return new ServiceResult<Permission>(
                    Success: false,
                    ResultType: ServiceResultType.Error,
                    Message: "Error al obtener el permiso.",
                    Data: null
                );
            }
        }

        public async Task<ServiceResult<Permission>> GetPermissionByNameAsync(string name, CancellationToken ct = default)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(name))
                {
                    return new ServiceResult<Permission>(
                        Success: false,
                        ResultType: ServiceResultType.Error,
                        Message: "Nombre de permiso inválido.",
                        Data: null
                    );
                }

                var permission = await _repo.GetByNameAsync(name);
                if (permission == null)
                {
                    return new ServiceResult<Permission>(
                        Success: false,
                        ResultType: ServiceResultType.NotFound,
                        Message: "Permiso no encontrado.",
                        Data: null
                    );
                }

                return new ServiceResult<Permission>(
                    Success: true,
                    ResultType: ServiceResultType.Success,
                    Message: "Permiso encontrado.",
                    Data: permission
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el permiso con nombre: {PermissionName}", name);
                return new ServiceResult<Permission>(
                    Success: false,
                    ResultType: ServiceResultType.Error,
                    Message: "Error al obtener el permiso.",
                    Data: null
                );
            }
        }

        public async Task<ServiceResult<IEnumerable<Permission>>> GetAllPermissionsAsync(CancellationToken ct = default)
        {
            try
            {
                var permissions = await _repo.GetAllAsync();
                return new ServiceResult<IEnumerable<Permission>>(
                    Success: true,
                    ResultType: ServiceResultType.Success,
                    Message: $"Se encontraron {permissions.Count()} permisos.",
                    Data: permissions
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los permisos");
                return new ServiceResult<IEnumerable<Permission>>(
                    Success: false,
                    ResultType: ServiceResultType.Error,
                    Message: "Error al obtener los permisos.",
                    Data: null
                );
            }
        }

        public async Task<ServiceResult<Permission>> UpdatePermissionAsync(Guid id, string name, string? description = null, CancellationToken ct = default)
        {
            try
            {
                if (id == Guid.Empty)
                {
                    return new ServiceResult<Permission>(
                        Success: false,
                        ResultType: ServiceResultType.Error,
                        Message: "ID de permiso inválido.",
                        Data: null
                    );
                }

                if (string.IsNullOrWhiteSpace(name))
                {
                    return new ServiceResult<Permission>(
                        Success: false,
                        ResultType: ServiceResultType.Error,
                        Message: "El nombre del permiso es requerido.",
                        Data: null
                    );
                }

                var permission = await _repo.GetByIdAsync(id);
                if (permission == null)
                {
                    return new ServiceResult<Permission>(
                        Success: false,
                        ResultType: ServiceResultType.NotFound,
                        Message: "Permiso no encontrado.",
                        Data: null
                    );
                }

                permission.UpdateDetails(name, description);
                await _repo.UpdateAsync(permission);

                _logger.LogInformation("Permiso actualizado exitosamente: {PermissionId}", id);

                return new ServiceResult<Permission>(
                    Success: true,
                    ResultType: ServiceResultType.Success,
                    Message: "Permiso actualizado exitosamente.",
                    Data: permission
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar el permiso: {PermissionId}", id);
                return new ServiceResult<Permission>(
                    Success: false,
                    ResultType: ServiceResultType.Error,
                    Message: "Error al actualizar el permiso.",
                    Data: null
                );
            }
        }

        public async Task<ServiceResult<bool>> DeletePermissionAsync(Guid id, CancellationToken ct = default)
        {
            try
            {
                if (id == Guid.Empty)
                {
                    return new ServiceResult<bool>(
                        Success: false,
                        ResultType: ServiceResultType.Error,
                        Message: "ID de permiso inválido.",
                        Data: false
                    );
                }

                var exists = await _repo.ExistsAsync(id);
                if (!exists)
                {
                    return new ServiceResult<bool>(
                        Success: false,
                        ResultType: ServiceResultType.NotFound,
                        Message: "Permiso no encontrado.",
                        Data: false
                    );
                }

                await _repo.DeleteAsync(id);

                _logger.LogInformation("Permiso eliminado exitosamente: {PermissionId}", id);

                return new ServiceResult<bool>(
                    Success: true,
                    ResultType: ServiceResultType.Success,
                    Message: "Permiso eliminado exitosamente.",
                    Data: true
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar el permiso: {PermissionId}", id);
                return new ServiceResult<bool>(
                    Success: false,
                    ResultType: ServiceResultType.Error,
                    Message: "Error al eliminar el permiso.",
                    Data: false
                );
            }
        }
    }
}

