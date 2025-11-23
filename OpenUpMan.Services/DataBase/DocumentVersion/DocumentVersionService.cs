using Microsoft.Extensions.Logging;
using OpenUpMan.Data;
using OpenUpMan.Domain;

namespace OpenUpMan.Services
{
    public class DocumentVersionService : IDocumentVersionService
    {
        private readonly IDocumentVersionRepository _repo;
        private readonly IDocumentRepository _documentRepo;
        private readonly ILogger<DocumentVersionService> _logger;

        public DocumentVersionService(
            IDocumentVersionRepository repo,
            IDocumentRepository documentRepo,
            ILogger<DocumentVersionService> logger)
        {
            _repo = repo;
            _documentRepo = documentRepo;
            _logger = logger;
        }

        public async Task<DocumentVersionServiceResult> CreateVersionAsync(Guid documentId, Guid createdBy, string filePath, string? observations = null, CancellationToken ct = default)
        {
            try
            {
                var document = await _documentRepo.GetByIdAsync(documentId, ct);
                if (document == null)
                {
                    return new DocumentVersionServiceResult(
                        Success: false,
                        ResultType: ServiceResultType.Error,
                        Message: "Documento no encontrado."
                    );
                }

                // Increment version number
                document.IncrementVersion();
                var versionNumber = document.LastVersionNumber;

                var version = new DocumentVersion(
                    documentId,
                    versionNumber,
                    createdBy,
                    filePath,
                    observations
                );

                await _repo.AddAsync(version, ct);
                await _documentRepo.UpdateAsync(document, ct);
                await _repo.SaveChangesAsync(ct);

                _logger.LogInformation("Versión creada: {VersionId} - V{VersionNumber} para documento {DocumentId}", 
                    version.Id, versionNumber, documentId);

                return new DocumentVersionServiceResult(
                    Success: true,
                    ResultType: ServiceResultType.Success,
                    Message: $"Versión {versionNumber} creada exitosamente.",
                    DocumentVersion: version
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear versión");
                return new DocumentVersionServiceResult(
                    Success: false,
                    ResultType: ServiceResultType.Error,
                    Message: "Error al crear la versión."
                );
            }
        }

        public async Task<DocumentVersionServiceResult> GetVersionByIdAsync(Guid id, CancellationToken ct = default)
        {
            try
            {
                var version = await _repo.GetByIdAsync(id, ct);
                if (version == null)
                {
                    return new DocumentVersionServiceResult(
                        Success: false,
                        ResultType: ServiceResultType.Error,
                        Message: "Versión no encontrada."
                    );
                }

                return new DocumentVersionServiceResult(
                    Success: true,
                    ResultType: ServiceResultType.Success,
                    Message: "Versión encontrada.",
                    DocumentVersion: version
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener versión");
                return new DocumentVersionServiceResult(
                    Success: false,
                    ResultType: ServiceResultType.Error,
                    Message: "Error al obtener la versión."
                );
            }
        }

        public async Task<IEnumerable<DocumentVersion>> GetVersionsByDocumentAsync(Guid documentId, CancellationToken ct = default)
        {
            return await _repo.GetByDocumentIdAsync(documentId, ct);
        }

        public async Task<DocumentVersionServiceResult> GetLatestVersionAsync(Guid documentId, CancellationToken ct = default)
        {
            try
            {
                var version = await _repo.GetLatestVersionAsync(documentId, ct);
                if (version == null)
                {
                    return new DocumentVersionServiceResult(
                        Success: false,
                        ResultType: ServiceResultType.Error,
                        Message: "No hay versiones para este documento."
                    );
                }

                return new DocumentVersionServiceResult(
                    Success: true,
                    ResultType: ServiceResultType.Success,
                    Message: "Última versión encontrada.",
                    DocumentVersion: version
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener última versión");
                return new DocumentVersionServiceResult(
                    Success: false,
                    ResultType: ServiceResultType.Error,
                    Message: "Error al obtener la última versión."
                );
            }
        }

        public async Task<DocumentVersionServiceResult> UpdateObservationsAsync(Guid id, string? observations, CancellationToken ct = default)
        {
            try
            {
                var version = await _repo.GetByIdAsync(id, ct);
                if (version == null)
                {
                    return new DocumentVersionServiceResult(
                        Success: false,
                        ResultType: ServiceResultType.Error,
                        Message: "Versión no encontrada."
                    );
                }

                version.UpdateObservations(observations);
                await _repo.UpdateAsync(version, ct);
                await _repo.SaveChangesAsync(ct);

                return new DocumentVersionServiceResult(
                    Success: true,
                    ResultType: ServiceResultType.Success,
                    Message: "Observaciones actualizadas exitosamente.",
                    DocumentVersion: version
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar observaciones");
                return new DocumentVersionServiceResult(
                    Success: false,
                    ResultType: ServiceResultType.Error,
                    Message: "Error al actualizar las observaciones."
                );
            }
        }
    }
}

