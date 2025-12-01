using OpenUpMan.Domain;

namespace OpenUpMan.Services
{
    public interface IDocumentVersionService
    {
        Task<DocumentVersionServiceResult> CreateVersionAsync(Guid documentId, Guid createdBy, string extension, byte[] binario, string? observations = null, CancellationToken ct = default);
        Task<DocumentVersionServiceResult> GetVersionByIdAsync(Guid id, CancellationToken ct = default);
        Task<IEnumerable<DocumentVersion>> GetVersionsByDocumentAsync(Guid documentId, CancellationToken ct = default);
        Task<DocumentVersionServiceResult> GetLatestVersionAsync(Guid documentId, CancellationToken ct = default);
        Task<DocumentVersionServiceResult> UpdateObservationsAsync(Guid id, string? observations, CancellationToken ct = default);
    }
}

