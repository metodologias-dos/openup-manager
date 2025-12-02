using Microsoft.EntityFrameworkCore;
using OpenUpMan.Domain;

namespace OpenUpMan.Data
{
    public class MicroincrementRepository : IMicroincrementRepository
    {
        private readonly AppDbContext _context;

        public MicroincrementRepository(AppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<Microincrement?> GetByIdAsync(int id, CancellationToken ct = default)
        {
            return await _context.Microincrements.FindAsync(new object[] { id }, ct);
        }

        public async Task<IEnumerable<Microincrement>> GetAllAsync(CancellationToken ct = default)
        {
            return await _context.Microincrements
                .OrderByDescending(m => m.Date)
                .ToListAsync(ct);
        }

        public async Task<IEnumerable<Microincrement>> GetByIterationIdAsync(int iterationId, CancellationToken ct = default)
        {
            return await _context.Microincrements
                .Where(m => m.IterationId == iterationId)
                .OrderByDescending(m => m.Date)
                .ToListAsync(ct);
        }

        public async Task<IEnumerable<Microincrement>> GetByAuthorIdAsync(int authorId, CancellationToken ct = default)
        {
            return await _context.Microincrements
                .Where(m => m.AuthorId == authorId)
                .OrderByDescending(m => m.Date)
                .ToListAsync(ct);
        }

        public async Task<IEnumerable<Microincrement>> GetByArtifactIdAsync(int artifactId, CancellationToken ct = default)
        {
            return await _context.Microincrements
                .Where(m => m.ArtifactId == artifactId)
                .OrderByDescending(m => m.Date)
                .ToListAsync(ct);
        }

        public async Task<IEnumerable<Microincrement>> GetByProjectIdAsync(int projectId, CancellationToken ct = default)
        {
            return await _context.Microincrements
                .Where(m => _context.Iterations.Any(i => i.Id == m.IterationId && 
                    _context.Phases.Any(p => p.Id == i.PhaseId && p.ProjectId == projectId)))
                .OrderByDescending(m => m.Date)
                .ToListAsync(ct);
        }

        public async Task AddAsync(Microincrement microincrement, CancellationToken ct = default)
        {
            if (microincrement == null)
                throw new ArgumentNullException(nameof(microincrement));

            await _context.Microincrements.AddAsync(microincrement, ct);
            await _context.SaveChangesAsync(ct);
        }

        public async Task UpdateAsync(Microincrement microincrement, CancellationToken ct = default)
        {
            if (microincrement == null)
                throw new ArgumentNullException(nameof(microincrement));

            _context.Microincrements.Update(microincrement);
            await _context.SaveChangesAsync(ct);
        }

        public async Task DeleteAsync(int id, CancellationToken ct = default)
        {
            var microincrement = await GetByIdAsync(id, ct);
            if (microincrement != null)
            {
                _context.Microincrements.Remove(microincrement);
                await _context.SaveChangesAsync(ct);
            }
        }

        public async Task<bool> ExistsAsync(int id, CancellationToken ct = default)
        {
            return await _context.Microincrements.AnyAsync(m => m.Id == id, ct);
        }
    }
}

