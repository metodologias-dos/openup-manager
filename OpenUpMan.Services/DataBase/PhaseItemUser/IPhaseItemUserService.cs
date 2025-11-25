using OpenUpMan.Domain;

namespace OpenUpMan.Services
{
    public interface IPhaseItemUserService
    {
        Task<PhaseItemUserServiceResult> AddUserToPhaseItemAsync(Guid phaseItemId, Guid userId, string role = "PARTICIPANT", CancellationToken ct = default);
        Task<PhaseItemUserServiceResult> RemoveUserFromPhaseItemAsync(Guid phaseItemId, Guid userId, CancellationToken ct = default);
        Task<PhaseItemUserServiceResult> ChangeUserRoleAsync(Guid phaseItemId, Guid userId, string newRole, CancellationToken ct = default);
        Task<IEnumerable<PhaseItemUser>> GetPhaseItemUsersAsync(Guid phaseItemId, CancellationToken ct = default);
        Task<IEnumerable<PhaseItemUser>> GetUserPhaseItemsAsync(Guid userId, CancellationToken ct = default);
    }
}

