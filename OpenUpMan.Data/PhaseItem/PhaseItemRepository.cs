using Microsoft.EntityFrameworkCore;
using OpenUpMan.Domain;

namespace OpenUpMan.Data
{
    public class PhaseItemRepository : IPhaseItemRepository
    {
        private readonly AppDbContext _ctx;

        public PhaseItemRepository(AppDbContext ctx)
        {
            _ctx = ctx;
        }

        public async Task<PhaseItem?> GetByIdAsync(Guid id, CancellationToken ct = default)
        {
            return await _ctx.PhaseItems
                .FirstOrDefaultAsync(pi => pi.Id == id, ct);
        }

        public async Task<IEnumerable<PhaseItem>> GetByPhaseIdAsync(Guid projectPhaseId, CancellationToken ct = default)
        {
            return await _ctx.PhaseItems
                .Where(pi => pi.ProjectPhaseId == projectPhaseId)
                .OrderBy(pi => pi.Number)
                .ToListAsync(ct);
        }

        public async Task<IEnumerable<PhaseItem>> GetIterationsByPhaseIdAsync(Guid projectPhaseId, CancellationToken ct = default)
        {
            return await _ctx.PhaseItems
                .Where(pi => pi.ProjectPhaseId == projectPhaseId && pi.Type == PhaseItemType.ITERATION)
                .OrderBy(pi => pi.Number)
                .ToListAsync(ct);
        }

        public async Task<IEnumerable<PhaseItem>> GetMicroincrementsByIterationIdAsync(Guid iterationId, CancellationToken ct = default)
        {
            return await _ctx.PhaseItems
                .Where(pi => pi.ParentIterationId == iterationId && pi.Type == PhaseItemType.MICROINCREMENT)
                .OrderBy(pi => pi.Number)
                .ToListAsync(ct);
        }

        public async Task<IEnumerable<PhaseItem>> GetByTypeAsync(Guid projectPhaseId, PhaseItemType type, CancellationToken ct = default)
        {
            return await _ctx.PhaseItems
                .Where(pi => pi.ProjectPhaseId == projectPhaseId && pi.Type == type)
                .OrderBy(pi => pi.Number)
                .ToListAsync(ct);
        }

        public async Task AddAsync(PhaseItem phaseItem, CancellationToken ct = default)
        {
            await _ctx.PhaseItems.AddAsync(phaseItem, ct);
        }

        public async Task UpdateAsync(PhaseItem phaseItem, CancellationToken ct = default)
        {
            _ctx.PhaseItems.Update(phaseItem);
            await Task.CompletedTask;
        }

        public async Task SaveChangesAsync(CancellationToken ct = default)
        {
            await _ctx.SaveChangesAsync(ct);
        }
    }
}

