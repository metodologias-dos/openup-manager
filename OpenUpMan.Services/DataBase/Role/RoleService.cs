using Microsoft.Extensions.Logging;
using OpenUpMan.Data.Repositories;
using OpenUpMan.Domain;

namespace OpenUpMan.Services
{
    public class RoleService : IRoleService
    {
        private readonly IRoleRepository _repo;
        private readonly ILogger<RoleService> _logger;

        public RoleService(IRoleRepository repo, ILogger<RoleService> logger)
        {
            _repo = repo ?? throw new ArgumentNullException(nameof(repo));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<ServiceResult<Role>> CreateRoleAsync(string name, string? description = null, CancellationToken ct = default)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(name))
                {
                    return new ServiceResult<Role>(
                        Success: false,
                        ResultType: ServiceResultType.Error,
                        Message: "El nombre del rol es requerido.",
                        Data: null
                    );
                }

                var existing = await _repo.GetByNameAsync(name);
                if (existing != null)
                {
                    return new ServiceResult<Role>(
                        Success: false,
                        ResultType: ServiceResultType.Error,
                        Message: $"Ya existe un rol con el nombre '{name}'.",
                        Data: null
                    );
                }

                var role = new Domain.Role(name, description);
                await _repo.AddAsync(role);

                _logger.LogInformation("Rol creado exitosamente: {RoleName} con ID: {RoleId}", name, role.Id);

                return new ServiceResult<Role>(
                    Success: true,
                    ResultType: ServiceResultType.Success,
                    Message: "Rol creado exitosamente.",
                    Data: role
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear el rol: {RoleName}", name);
                return new ServiceResult<Role>(
                    Success: false,
                    ResultType: ServiceResultType.Error,
                    Message: "Error al crear el rol. Por favor, inténtelo de nuevo.",
                    Data: null
                );
            }
        }

        public async Task<ServiceResult<Role>> GetRoleByIdAsync(Guid id, CancellationToken ct = default)
        {
            try
            {
                if (id == Guid.Empty)
                {
                    return new ServiceResult<Role>(
                        Success: false,
                        ResultType: ServiceResultType.Error,
                        Message: "ID de rol inválido.",
                        Data: null
                    );
                }

                var role = await _repo.GetByIdAsync(id);
                if (role == null)
                {
                    return new ServiceResult<Role>(
                        Success: false,
                        ResultType: ServiceResultType.NotFound,
                        Message: "Rol no encontrado.",
                        Data: null
                    );
                }

                return new ServiceResult<Role>(
                    Success: true,
                    ResultType: ServiceResultType.Success,
                    Message: "Rol encontrado.",
                    Data: role
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el rol con ID: {RoleId}", id);
                return new ServiceResult<Role>(
                    Success: false,
                    ResultType: ServiceResultType.Error,
                    Message: "Error al obtener el rol.",
                    Data: null
                );
            }
        }

        public async Task<ServiceResult<Role>> GetRoleByNameAsync(string name, CancellationToken ct = default)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(name))
                {
                    return new ServiceResult<Role>(
                        Success: false,
                        ResultType: ServiceResultType.Error,
                        Message: "Nombre de rol inválido.",
                        Data: null
                    );
                }

                var role = await _repo.GetByNameAsync(name);
                if (role == null)
                {
                    return new ServiceResult<Role>(
                        Success: false,
                        ResultType: ServiceResultType.NotFound,
                        Message: "Rol no encontrado.",
                        Data: null
                    );
                }

                return new ServiceResult<Role>(
                    Success: true,
                    ResultType: ServiceResultType.Success,
                    Message: "Rol encontrado.",
                    Data: role
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el rol con nombre: {RoleName}", name);
                return new ServiceResult<Role>(
                    Success: false,
                    ResultType: ServiceResultType.Error,
                    Message: "Error al obtener el rol.",
                    Data: null
                );
            }
        }

        public async Task<ServiceResult<IEnumerable<Role>>> GetAllRolesAsync(CancellationToken ct = default)
        {
            try
            {
                var roles = await _repo.GetAllAsync();
                return new ServiceResult<IEnumerable<Role>>(
                    Success: true,
                    ResultType: ServiceResultType.Success,
                    Message: $"Se encontraron {roles.Count()} roles.",
                    Data: roles
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los roles");
                return new ServiceResult<IEnumerable<Role>>(
                    Success: false,
                    ResultType: ServiceResultType.Error,
                    Message: "Error al obtener los roles.",
                    Data: null
                );
            }
        }

        public async Task<ServiceResult<Role>> UpdateRoleAsync(Guid id, string name, string? description = null, CancellationToken ct = default)
        {
            try
            {
                if (id == Guid.Empty)
                {
                    return new ServiceResult<Role>(
                        Success: false,
                        ResultType: ServiceResultType.Error,
                        Message: "ID de rol inválido.",
                        Data: null
                    );
                }

                if (string.IsNullOrWhiteSpace(name))
                {
                    return new ServiceResult<Role>(
                        Success: false,
                        ResultType: ServiceResultType.Error,
                        Message: "El nombre del rol es requerido.",
                        Data: null
                    );
                }

                var role = await _repo.GetByIdAsync(id);
                if (role == null)
                {
                    return new ServiceResult<Role>(
                        Success: false,
                        ResultType: ServiceResultType.NotFound,
                        Message: "Rol no encontrado.",
                        Data: null
                    );
                }

                role.UpdateDetails(name, description);
                await _repo.UpdateAsync(role);

                _logger.LogInformation("Rol actualizado exitosamente: {RoleId}", id);

                return new ServiceResult<Role>(
                    Success: true,
                    ResultType: ServiceResultType.Success,
                    Message: "Rol actualizado exitosamente.",
                    Data: role
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar el rol: {RoleId}", id);
                return new ServiceResult<Role>(
                    Success: false,
                    ResultType: ServiceResultType.Error,
                    Message: "Error al actualizar el rol.",
                    Data: null
                );
            }
        }

        public async Task<ServiceResult<bool>> DeleteRoleAsync(Guid id, CancellationToken ct = default)
        {
            try
            {
                if (id == Guid.Empty)
                {
                    return new ServiceResult<bool>(
                        Success: false,
                        ResultType: ServiceResultType.Error,
                        Message: "ID de rol inválido.",
                        Data: false
                    );
                }

                var exists = await _repo.ExistsAsync(id);
                if (!exists)
                {
                    return new ServiceResult<bool>(
                        Success: false,
                        ResultType: ServiceResultType.NotFound,
                        Message: "Rol no encontrado.",
                        Data: false
                    );
                }

                await _repo.DeleteAsync(id);

                _logger.LogInformation("Rol eliminado exitosamente: {RoleId}", id);

                return new ServiceResult<bool>(
                    Success: true,
                    ResultType: ServiceResultType.Success,
                    Message: "Rol eliminado exitosamente.",
                    Data: true
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar el rol: {RoleId}", id);
                return new ServiceResult<bool>(
                    Success: false,
                    ResultType: ServiceResultType.Error,
                    Message: "Error al eliminar el rol.",
                    Data: false
                );
            }
        }
    }
}

