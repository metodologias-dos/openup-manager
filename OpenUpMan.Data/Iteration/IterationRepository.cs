using Microsoft.EntityFrameworkCore;
using OpenUpMan.Domain;

namespace OpenUpMan.Data
{
    public class IterationRepository : IIterationRepository
    {
        private readonly AppDbContext _context;

        public IterationRepository(AppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<Iteration?> GetByIdAsync(int id, CancellationToken ct = default)
        {
            return await _context.Iterations.FindAsync(new object[] { id }, ct);
        }

        public async Task<IEnumerable<Iteration>> GetAllAsync(CancellationToken ct = default)
        {
            return await _context.Iterations.ToListAsync(ct);
        }

        public async Task<IEnumerable<Iteration>> GetByPhaseIdAsync(int phaseId, CancellationToken ct = default)
        {
            return await _context.Iterations
                .Where(i => i.PhaseId == phaseId)
                .OrderBy(i => i.StartDate)
                .ToListAsync(ct);
        }

        public async Task<IEnumerable<Iteration>> GetByProjectIdAsync(int projectId, CancellationToken ct = default)
        {
            return await _context.Iterations
                .Where(i => _context.Phases.Any(p => p.Id == i.PhaseId && p.ProjectId == projectId))
                .OrderBy(i => i.StartDate)
                .ToListAsync(ct);
        }

        public async Task AddAsync(Iteration iteration, CancellationToken ct = default)
        {
            if (iteration == null)
                throw new ArgumentNullException(nameof(iteration));

            await _context.Iterations.AddAsync(iteration, ct);
            await _context.SaveChangesAsync(ct);
        }

        public async Task UpdateAsync(Iteration iteration, CancellationToken ct = default)
        {
            if (iteration == null)
                throw new ArgumentNullException(nameof(iteration));

            _context.Iterations.Update(iteration);
            await _context.SaveChangesAsync(ct);
        }

        public async Task DeleteAsync(int id, CancellationToken ct = default)
        {
            var iteration = await GetByIdAsync(id, ct);
            if (iteration != null)
            {
                _context.Iterations.Remove(iteration);
                await _context.SaveChangesAsync(ct);
            }
        }

        public async Task<bool> ExistsAsync(int id, CancellationToken ct = default)
        {
            return await _context.Iterations.AnyAsync(i => i.Id == id, ct);
        }
    }
}

