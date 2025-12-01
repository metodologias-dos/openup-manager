using OpenUpMan.Domain;

namespace OpenUpMan.Services
{
    public interface IDocumentService
    {
        Task<DocumentServiceResult> CreateDocumentAsync(Guid phaseItemId, string title, string? description = null, CancellationToken ct = default);
        Task<DocumentServiceResult> GetDocumentByIdAsync(Guid id, CancellationToken ct = default);
        Task<IEnumerable<Document>> GetDocumentsByPhaseItemAsync(Guid phaseItemId, CancellationToken ct = default);
        Task<DocumentServiceResult> UpdateDocumentAsync(Guid id, string title, string? description, CancellationToken ct = default);
    }
}

