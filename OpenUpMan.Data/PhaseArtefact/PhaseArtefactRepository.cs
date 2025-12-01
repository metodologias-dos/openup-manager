using Microsoft.EntityFrameworkCore;
using OpenUpMan.Domain;

namespace OpenUpMan.Data
{
    public class PhaseArtefactRepository : IPhaseArtefactRepository
    {
        private readonly AppDbContext _context;

        public PhaseArtefactRepository(AppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<IEnumerable<Artefact>> GetArtefactsByPhaseIdAsync(Guid phaseId)
        {
            var phaseArtefacts = await _context.PhaseArtefacts
                .Where(pa => pa.PhaseId == phaseId)
                .ToListAsync();
            
            var artefactIds = phaseArtefacts.Select(pa => pa.ArtefactId);
            return await _context.Artefacts
                .Where(a => artefactIds.Contains(a.Id))
                .ToListAsync();
        }

        public async Task<IEnumerable<PhaseArtefact>> GetByPhaseIdAsync(Guid phaseId)
        {
            return await _context.PhaseArtefacts
                .Where(pa => pa.PhaseId == phaseId)
                .ToListAsync();
        }

        public async Task<PhaseArtefact?> GetByIdAsync(Guid phaseId, Guid artefactId)
        {
            return await _context.PhaseArtefacts
                .FirstOrDefaultAsync(pa => pa.PhaseId == phaseId && pa.ArtefactId == artefactId);
        }

        public async Task AddAsync(PhaseArtefact phaseArtefact)
        {
            if (phaseArtefact == null)
                throw new ArgumentNullException(nameof(phaseArtefact));

            await _context.PhaseArtefacts.AddAsync(phaseArtefact);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(PhaseArtefact phaseArtefact)
        {
            if (phaseArtefact == null)
                throw new ArgumentNullException(nameof(phaseArtefact));

            _context.PhaseArtefacts.Update(phaseArtefact);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid phaseId, Guid artefactId)
        {
            var phaseArtefact = await GetByIdAsync(phaseId, artefactId);
            if (phaseArtefact != null)
            {
                _context.PhaseArtefacts.Remove(phaseArtefact);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ExistsAsync(Guid phaseId, Guid artefactId)
        {
            return await _context.PhaseArtefacts
                .AnyAsync(pa => pa.PhaseId == phaseId && pa.ArtefactId == artefactId);
        }
    }
}

