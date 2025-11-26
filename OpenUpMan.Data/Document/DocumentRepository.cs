using Microsoft.EntityFrameworkCore;
using OpenUpMan.Domain;

namespace OpenUpMan.Data
{
    public class DocumentRepository : IDocumentRepository
    {
        private readonly AppDbContext _ctx;

        public DocumentRepository(AppDbContext ctx)
        {
            _ctx = ctx;
        }

        public async Task<Document?> GetByIdAsync(Guid id, CancellationToken ct = default)
        {
            return await _ctx.Documents
                .Include(d => d.PhaseItem)
                .Include(d => d.Creator)
                .FirstOrDefaultAsync(d => d.Id == id, ct);
        }

        public async Task<IEnumerable<Document>> GetByPhaseItemIdAsync(Guid phaseItemId, CancellationToken ct = default)
        {
            return await _ctx.Documents
                .Include(d => d.Creator)
                .Where(d => d.PhaseItemId == phaseItemId)
                .OrderByDescending(d => d.CreatedAt)
                .ToListAsync(ct);
        }

        public async Task<IEnumerable<Document>> GetByCreatorIdAsync(Guid creatorId, CancellationToken ct = default)
        {
            return await _ctx.Documents
                .Include(d => d.PhaseItem)
                .Where(d => d.CreatedBy == creatorId)
                .OrderByDescending(d => d.CreatedAt)
                .ToListAsync(ct);
        }

        public async Task AddAsync(Document document, CancellationToken ct = default)
        {
            await _ctx.Documents.AddAsync(document, ct);
        }

        public async Task UpdateAsync(Document document, CancellationToken ct = default)
        {
            _ctx.Documents.Update(document);
            await Task.CompletedTask;
        }

        public async Task SaveChangesAsync(CancellationToken ct = default)
        {
            await _ctx.SaveChangesAsync(ct);
        }
    }
}

