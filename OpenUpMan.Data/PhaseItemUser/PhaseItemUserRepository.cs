using Microsoft.EntityFrameworkCore;
using OpenUpMan.Domain;

namespace OpenUpMan.Data
{
    public class PhaseItemUserRepository : IPhaseItemUserRepository
    {
        private readonly AppDbContext _ctx;

        public PhaseItemUserRepository(AppDbContext ctx)
        {
            _ctx = ctx;
        }

        public async Task<PhaseItemUser?> GetByIdAsync(Guid phaseItemId, Guid userId, CancellationToken ct = default)
        {
            return await _ctx.PhaseItemUsers
                .FirstOrDefaultAsync(piu => piu.PhaseItemId == phaseItemId && piu.UserId == userId, ct);
        }

        public async Task<IEnumerable<PhaseItemUser>> GetByPhaseItemIdAsync(Guid phaseItemId, CancellationToken ct = default)
        {
            return await _ctx.PhaseItemUsers
                .Where(piu => piu.PhaseItemId == phaseItemId)
                .ToListAsync(ct);
        }

        public async Task<IEnumerable<PhaseItemUser>> GetByUserIdAsync(Guid userId, CancellationToken ct = default)
        {
            return await _ctx.PhaseItemUsers
                .Where(piu => piu.UserId == userId)
                .ToListAsync(ct);
        }

        public async Task AddAsync(PhaseItemUser phaseItemUser, CancellationToken ct = default)
        {
            await _ctx.PhaseItemUsers.AddAsync(phaseItemUser, ct);
        }

        public async Task RemoveAsync(PhaseItemUser phaseItemUser, CancellationToken ct = default)
        {
            _ctx.PhaseItemUsers.Remove(phaseItemUser);
            await Task.CompletedTask;
        }

        public async Task SaveChangesAsync(CancellationToken ct = default)
        {
            await _ctx.SaveChangesAsync(ct);
        }
    }
}

