using Microsoft.EntityFrameworkCore;
using OpenUpMan.Domain;

namespace OpenUpMan.Data
{
    public class DocumentVersionRepository : IDocumentVersionRepository
    {
        private readonly AppDbContext _ctx;

        public DocumentVersionRepository(AppDbContext ctx)
        {
            _ctx = ctx;
        }

        public async Task<DocumentVersion?> GetByIdAsync(Guid id, CancellationToken ct = default)
        {
            return await _ctx.DocumentVersions
                .FirstOrDefaultAsync(dv => dv.Id == id, ct);
        }

        public async Task<IEnumerable<DocumentVersion>> GetByDocumentIdAsync(Guid documentId, CancellationToken ct = default)
        {
            return await _ctx.DocumentVersions
                .Where(dv => dv.DocumentId == documentId)
                .OrderBy(dv => dv.VersionNumber)
                .ToListAsync(ct);
        }

        public async Task<DocumentVersion?> GetByDocumentIdAndVersionAsync(Guid documentId, int versionNumber, CancellationToken ct = default)
        {
            return await _ctx.DocumentVersions
                .FirstOrDefaultAsync(dv => dv.DocumentId == documentId && dv.VersionNumber == versionNumber, ct);
        }

        public async Task<DocumentVersion?> GetLatestVersionAsync(Guid documentId, CancellationToken ct = default)
        {
            return await _ctx.DocumentVersions
                .Where(dv => dv.DocumentId == documentId)
                .OrderByDescending(dv => dv.VersionNumber)
                .FirstOrDefaultAsync(ct);
        }

        public async Task AddAsync(DocumentVersion documentVersion, CancellationToken ct = default)
        {
            await _ctx.DocumentVersions.AddAsync(documentVersion, ct);
        }

        public async Task UpdateAsync(DocumentVersion documentVersion, CancellationToken ct = default)
        {
            _ctx.DocumentVersions.Update(documentVersion);
            await Task.CompletedTask;
        }

        public async Task SaveChangesAsync(CancellationToken ct = default)
        {
            await _ctx.SaveChangesAsync(ct);
        }
    }
}

