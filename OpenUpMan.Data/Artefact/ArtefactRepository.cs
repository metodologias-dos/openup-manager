using Microsoft.EntityFrameworkCore;
using OpenUpMan.Domain;

namespace OpenUpMan.Data.Repositories
{
    public class ArtefactRepository : IArtefactRepository
    {
        private readonly AppDbContext _context;

        public ArtefactRepository(AppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<Artefact?> GetByIdAsync(Guid id)
        {
            return await _context.Artefacts.FindAsync(id);
        }

        public async Task<Artefact?> GetByNameAsync(string name)
        {
            return await _context.Artefacts
                .FirstOrDefaultAsync(a => a.Name == name);
        }

        public async Task<IEnumerable<Artefact>> GetAllAsync()
        {
            return await _context.Artefacts.ToListAsync();
        }

        public async Task AddAsync(Artefact artefact)
        {
            if (artefact == null)
                throw new ArgumentNullException(nameof(artefact));

            await _context.Artefacts.AddAsync(artefact);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Artefact artefact)
        {
            if (artefact == null)
                throw new ArgumentNullException(nameof(artefact));

            _context.Artefacts.Update(artefact);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var artefact = await GetByIdAsync(id);
            if (artefact != null)
            {
                _context.Artefacts.Remove(artefact);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ExistsAsync(Guid id)
        {
            return await _context.Artefacts.AnyAsync(a => a.Id == id);
        }
    }
}

