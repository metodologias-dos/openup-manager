using OpenUpMan.Domain;

namespace OpenUpMan.Data
{
    public interface IDocumentVersionRepository
    {
        Task<DocumentVersion?> GetByIdAsync(Guid id, CancellationToken ct = default);
        Task<IEnumerable<DocumentVersion>> GetByDocumentIdAsync(Guid documentId, CancellationToken ct = default);
        Task<DocumentVersion?> GetByDocumentIdAndVersionAsync(Guid documentId, int versionNumber, CancellationToken ct = default);
        Task<DocumentVersion?> GetLatestVersionAsync(Guid documentId, CancellationToken ct = default);
        Task AddAsync(DocumentVersion documentVersion, CancellationToken ct = default);
        Task UpdateAsync(DocumentVersion documentVersion, CancellationToken ct = default);
        Task SaveChangesAsync(CancellationToken ct = default);
    }
}

