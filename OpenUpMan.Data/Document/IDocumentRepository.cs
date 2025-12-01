using OpenUpMan.Domain;

namespace OpenUpMan.Data
{
    public interface IDocumentRepository
    {
        Task<Document?> GetByIdAsync(Guid id, CancellationToken ct = default);
        Task<IEnumerable<Document>> GetByPhaseItemIdAsync(Guid phaseItemId, CancellationToken ct = default);
        Task AddAsync(Document document, CancellationToken ct = default);
        Task UpdateAsync(Document document, CancellationToken ct = default);
        Task SaveChangesAsync(CancellationToken ct = default);
    }
}

