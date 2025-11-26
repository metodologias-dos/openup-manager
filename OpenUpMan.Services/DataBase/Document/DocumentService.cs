using Microsoft.Extensions.Logging;
using OpenUpMan.Data;
using OpenUpMan.Domain;

namespace OpenUpMan.Services
{
    public class DocumentService : IDocumentService
    {
        private readonly IDocumentRepository _repo;
        private readonly IPhaseItemRepository _itemRepo;
        private readonly ILogger<DocumentService> _logger;

        public DocumentService(
            IDocumentRepository repo,
            IPhaseItemRepository itemRepo,
            ILogger<DocumentService> logger)
        {
            _repo = repo;
            _itemRepo = itemRepo;
            _logger = logger;
        }

        public async Task<DocumentServiceResult> CreateDocumentAsync(Guid phaseItemId, string title, Guid createdBy, string? description = null, CancellationToken ct = default)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(title))
                {
                    return new DocumentServiceResult(
                        Success: false,
                        ResultType: ServiceResultType.Error,
                        Message: "El título es requerido."
                    );
                }

                var document = new Document(phaseItemId, title, createdBy, description);
                await _repo.AddAsync(document, ct);
                await _repo.SaveChangesAsync(ct);

                _logger.LogInformation("Documento creado: {DocumentId} - {Title}", document.Id, title);

                return new DocumentServiceResult(
                    Success: true,
                    ResultType: ServiceResultType.Success,
                    Message: "Documento creado exitosamente.",
                    Document: document
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear documento");
                return new DocumentServiceResult(
                    Success: false,
                    ResultType: ServiceResultType.Error,
                    Message: "Error al crear el documento."
                );
            }
        }

        public async Task<DocumentServiceResult> GetDocumentByIdAsync(Guid id, CancellationToken ct = default)
        {
            try
            {
                var document = await _repo.GetByIdAsync(id, ct);
                if (document == null)
                {
                    return new DocumentServiceResult(
                        Success: false,
                        ResultType: ServiceResultType.Error,
                        Message: "Documento no encontrado."
                    );
                }

                return new DocumentServiceResult(
                    Success: true,
                    ResultType: ServiceResultType.Success,
                    Message: "Documento encontrado.",
                    Document: document
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener documento");
                return new DocumentServiceResult(
                    Success: false,
                    ResultType: ServiceResultType.Error,
                    Message: "Error al obtener el documento."
                );
            }
        }

        public async Task<IEnumerable<Document>> GetDocumentsByPhaseItemAsync(Guid phaseItemId, CancellationToken ct = default)
        {
            return await _repo.GetByPhaseItemIdAsync(phaseItemId, ct);
        }

        public async Task<IEnumerable<Document>> GetDocumentsByCreatorAsync(Guid creatorId, CancellationToken ct = default)
        {
            return await _repo.GetByCreatorIdAsync(creatorId, ct);
        }

        public async Task<DocumentServiceResult> UpdateDocumentAsync(Guid id, string title, string? description, CancellationToken ct = default)
        {
            try
            {
                var document = await _repo.GetByIdAsync(id, ct);
                if (document == null)
                {
                    return new DocumentServiceResult(
                        Success: false,
                        ResultType: ServiceResultType.Error,
                        Message: "Documento no encontrado."
                    );
                }

                document.UpdateDetails(title, description);
                await _repo.UpdateAsync(document, ct);
                await _repo.SaveChangesAsync(ct);

                return new DocumentServiceResult(
                    Success: true,
                    ResultType: ServiceResultType.Success,
                    Message: "Documento actualizado exitosamente.",
                    Document: document
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar documento");
                return new DocumentServiceResult(
                    Success: false,
                    ResultType: ServiceResultType.Error,
                    Message: "Error al actualizar el documento."
                );
            }
        }
    }
}

