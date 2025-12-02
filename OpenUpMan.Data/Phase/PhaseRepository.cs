using Microsoft.EntityFrameworkCore;
using OpenUpMan.Domain;

namespace OpenUpMan.Data
{
    public class PhaseRepository : IPhaseRepository
    {
        private readonly AppDbContext _context;

        public PhaseRepository(AppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<Phase?> GetByIdAsync(int id, CancellationToken ct = default)
        {
            return await _context.Phases.FindAsync(new object[] { id }, ct);
        }

        public async Task<IEnumerable<Phase>> GetAllAsync(CancellationToken ct = default)
        {
            return await _context.Phases
                .OrderBy(p => p.OrderIndex)
                .ToListAsync(ct);
        }

        public async Task<IEnumerable<Phase>> GetByProjectIdAsync(int projectId, CancellationToken ct = default)
        {
            return await _context.Phases
                .Where(p => p.ProjectId == projectId)
                .OrderBy(p => p.OrderIndex)
                .ToListAsync(ct);
        }

        public async Task AddAsync(Phase phase, CancellationToken ct = default)
        {
            if (phase == null)
                throw new ArgumentNullException(nameof(phase));

            await _context.Phases.AddAsync(phase, ct);
            await _context.SaveChangesAsync(ct);
        }

        public async Task UpdateAsync(Phase phase, CancellationToken ct = default)
        {
            if (phase == null)
                throw new ArgumentNullException(nameof(phase));

            _context.Phases.Update(phase);
            await _context.SaveChangesAsync(ct);
        }

        public async Task DeleteAsync(int id, CancellationToken ct = default)
        {
            var phase = await GetByIdAsync(id, ct);
            if (phase != null)
            {
                _context.Phases.Remove(phase);
                await _context.SaveChangesAsync(ct);
            }
        }

        public async Task<bool> ExistsAsync(int id, CancellationToken ct = default)
        {
            return await _context.Phases.AnyAsync(p => p.Id == id, ct);
        }
    }
}

