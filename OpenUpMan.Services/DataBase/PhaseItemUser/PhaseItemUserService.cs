using Microsoft.Extensions.Logging;
using OpenUpMan.Data;
using OpenUpMan.Domain;

namespace OpenUpMan.Services
{
    public class PhaseItemUserService : IPhaseItemUserService
    {
        private readonly IPhaseItemUserRepository _repo;
        private readonly IPhaseItemRepository _itemRepo;
        private readonly IUserRepository _userRepo;
        private readonly ILogger<PhaseItemUserService> _logger;

        public PhaseItemUserService(
            IPhaseItemUserRepository repo,
            IPhaseItemRepository itemRepo,
            IUserRepository userRepo,
            ILogger<PhaseItemUserService> logger)
        {
            _repo = repo;
            _itemRepo = itemRepo;
            _userRepo = userRepo;
            _logger = logger;
        }

        public async Task<PhaseItemUserServiceResult> AddUserToPhaseItemAsync(Guid phaseItemId, Guid userId, string role = "PARTICIPANT", CancellationToken ct = default)
        {
            try
            {
                var existing = await _repo.GetByIdAsync(phaseItemId, userId, ct);
                if (existing != null)
                {
                    return new PhaseItemUserServiceResult(
                        Success: false,
                        ResultType: ServiceResultType.Error,
                        Message: "El usuario ya está asignado a este item."
                    );
                }

                var phaseItemUser = new PhaseItemUser(phaseItemId, userId, role);
                await _repo.AddAsync(phaseItemUser, ct);
                await _repo.SaveChangesAsync(ct);

                _logger.LogInformation("Usuario {UserId} agregado al item {ItemId} con rol {Role}", userId, phaseItemId, role);

                return new PhaseItemUserServiceResult(
                    Success: true,
                    ResultType: ServiceResultType.Success,
                    Message: "Usuario agregado al item exitosamente.",
                    PhaseItemUser: phaseItemUser
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al agregar usuario al item");
                return new PhaseItemUserServiceResult(
                    Success: false,
                    ResultType: ServiceResultType.Error,
                    Message: "Error al agregar usuario al item."
                );
            }
        }

        public async Task<PhaseItemUserServiceResult> RemoveUserFromPhaseItemAsync(Guid phaseItemId, Guid userId, CancellationToken ct = default)
        {
            try
            {
                var phaseItemUser = await _repo.GetByIdAsync(phaseItemId, userId, ct);
                if (phaseItemUser == null)
                {
                    return new PhaseItemUserServiceResult(
                        Success: false,
                        ResultType: ServiceResultType.Error,
                        Message: "El usuario no está asignado a este item."
                    );
                }

                await _repo.RemoveAsync(phaseItemUser, ct);
                await _repo.SaveChangesAsync(ct);

                return new PhaseItemUserServiceResult(
                    Success: true,
                    ResultType: ServiceResultType.Success,
                    Message: "Usuario removido del item exitosamente."
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al remover usuario del item");
                return new PhaseItemUserServiceResult(
                    Success: false,
                    ResultType: ServiceResultType.Error,
                    Message: "Error al remover usuario del item."
                );
            }
        }

        public async Task<PhaseItemUserServiceResult> ChangeUserRoleAsync(Guid phaseItemId, Guid userId, string newRole, CancellationToken ct = default)
        {
            try
            {
                var phaseItemUser = await _repo.GetByIdAsync(phaseItemId, userId, ct);
                if (phaseItemUser == null)
                {
                    return new PhaseItemUserServiceResult(
                        Success: false,
                        ResultType: ServiceResultType.Error,
                        Message: "El usuario no está asignado a este item."
                    );
                }

                phaseItemUser.SetRole(newRole);
                await _repo.SaveChangesAsync(ct);

                return new PhaseItemUserServiceResult(
                    Success: true,
                    ResultType: ServiceResultType.Success,
                    Message: "Rol del usuario actualizado exitosamente.",
                    PhaseItemUser: phaseItemUser
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cambiar rol del usuario");
                return new PhaseItemUserServiceResult(
                    Success: false,
                    ResultType: ServiceResultType.Error,
                    Message: "Error al cambiar el rol del usuario."
                );
            }
        }

        public async Task<IEnumerable<PhaseItemUser>> GetPhaseItemUsersAsync(Guid phaseItemId, CancellationToken ct = default)
        {
            return await _repo.GetByPhaseItemIdAsync(phaseItemId, ct);
        }

        public async Task<IEnumerable<PhaseItemUser>> GetUserPhaseItemsAsync(Guid userId, CancellationToken ct = default)
        {
            return await _repo.GetByUserIdAsync(userId, ct);
        }
    }
}

